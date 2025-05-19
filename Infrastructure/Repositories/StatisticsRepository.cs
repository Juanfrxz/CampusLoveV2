using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using MySql.Data.MySqlClient;
using Dapper;

namespace CampusLove.Infrastructure.Repositories
{
    public class StatisticsRepository
    {
        private readonly MySqlConnection _connection;
        private readonly UserRepository _userRepository;
        private readonly UserLikesRepository _userLikesRepository;
        private readonly InterestProfileRepository _interestProfileRepository;
        private readonly UserMatchRepository _userMatchRepository;
        private readonly GenderRepository _genderRepository;

        public StatisticsRepository(MySqlConnection connection)
        {
            _connection = connection;
            _userRepository = new UserRepository(connection);
            _userLikesRepository = new UserLikesRepository(connection);
            _interestProfileRepository = new InterestProfileRepository(connection);
            _userMatchRepository = new UserMatchRepository(connection);
            _genderRepository = new GenderRepository(connection);
        }

        public async Task<MaxLikes> GetUserWithMostLikes()
        {
            const string sql = @"
                SELECT u.username, u.profile_id, COUNT(ul.id) as TotalLikes
                FROM user u
                LEFT JOIN userlikes ul ON u.id = ul.user_id
                GROUP BY u.id
                ORDER BY TotalLikes DESC
                LIMIT 1";

            return await _connection.QueryFirstOrDefaultAsync<MaxLikes>(sql);
        }

        public async Task<LessLikes> GetUserLessLikes()
        {
            const string sql = @"
                SELECT u.username, u.profile_id, COUNT(ul.id) as TotalLikes
                FROM user u
                LEFT JOIN userlikes ul ON u.id = ul.user_id
                GROUP BY u.id
                ORDER BY TotalLikes ASC
                LIMIT 1";

            return await _connection.QueryFirstOrDefaultAsync<LessLikes>(sql);
        }

        public async Task<IEnumerable<UserInterest>> GetMostPopularInterests(int take = 5)
        {
            const string sql = @"
                SELECT i.*, COUNT(ip.profile_id) as UsersCount
                FROM interest i
                JOIN interestProfile ip ON i.id = ip.interest_id
                GROUP BY i.id
                ORDER BY UsersCount DESC
                LIMIT @Take";

            return await _connection.QueryAsync<UserInterest>(sql, new { Take = take });
        }

        public async Task<IEnumerable<GenderMatchStats>> GetMatchesByGender()
        {
            const string sql = @"
                SELECT g.description as Gender, COUNT(um.id) as MatchCount
                FROM user_match um
                JOIN user u ON um.user1_id = u.id
                JOIN profile p ON u.profile_id = p.id
                JOIN gender g ON p.gender_id = g.id
                GROUP BY g.description";

            return await _connection.QueryAsync<GenderMatchStats>(sql);
        }
    }
}