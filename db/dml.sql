INSERT INTO profession (description) VALUES ('Ingeniería de Sistemas'), ('Medicina'), ('Psicología');
INSERT INTO gender (description) VALUES ('Masculino'), ('Femenino'), ('No binario');
INSERT INTO status (description) VALUES ('Soltero'), ('Relación abierta'), ('Casado');
INSERT INTO interest (description) VALUES ('Música'), ('Cine'), ('Viajes');

INSERT INTO profile (name, lastname, identification, gender_id, slogan, status_id, profession_id) 
VALUES ('María', 'Rodríguez', '1098765432', 1, 'La vida es una aventura', 1, 1);

INSERT INTO profile (name, lastname, identification, gender_id, slogan, status_id, profession_id) 
VALUES ('Juan', 'Martínez', '1087654321', 2, 'Amante de la naturaleza', 3, 2);

INSERT INTO profile (name, lastname, identification, gender_id, slogan, status_id, profession_id) 
VALUES ('Laura', 'Diaz', '1076543210', 3, 'Explorando nuevos horizontes', 2, 3);

-- Inserts para la tabla user
INSERT INTO user (username, password, profile_id, birthdate) 
VALUES ('maria_r', 'password123', 1, '1995-05-10');

INSERT INTO user (username, password, profile_id, birthdate) 
VALUES ('juan_m', 'securepass456', 2, '1997-08-22');

INSERT INTO user (username, password, profile_id, birthdate) 
VALUES ('laura_d', 'mypass789', 3, '1990-12-15');

INSERT INTO reaction (user_id, profile_id, reaction_type) 
VALUES (1, 2, 'like');

INSERT INTO reaction (user_id, profile_id, reaction_type) 
VALUES (2, 1, 'like');

INSERT INTO reaction (user_id, profile_id, reaction_type) 
VALUES (3, 1, 'dislike');

INSERT INTO interestProfile (profile_id, interest_id) 
VALUES (1, 1), (2, 2), (3,3);
INSERT INTO user_match (user1_id, user2_id) 
VALUES (1, 2);

INSERT INTO user_match (user1_id, user2_id) 
VALUES (2, 3);

INSERT INTO user_match (user1_id, user2_id) 
VALUES (3, 1);

INSERT INTO daily_likes (date, profile_id, number_likes, status) 
VALUES (CURDATE(), 1, 8, TRUE);

INSERT INTO daily_likes (date, profile_id, number_likes, status) 
VALUES (CURDATE(), 2, 7, TRUE);

INSERT INTO daily_likes (date, profile_id, number_likes, status) 
VALUES (CURDATE(), 3, 10, FALSE);