DROP DATABASE IF EXISTS campusLove;
CREATE DATABASE IF NOT EXISTS campusLove;
USE campusLove;

CREATE TABLE IF NOT EXISTS profession (
    id INT PRIMARY KEY AUTO_INCREMENT,
    description VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS gender (
    id INT PRIMARY KEY AUTO_INCREMENT,
    description VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS status (
    id INT PRIMARY KEY AUTO_INCREMENT,
    description VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS interest (
    id INT PRIMARY KEY AUTO_INCREMENT,
    description VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS application (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50) UNIQUE,
    description VARCHAR(100),
    createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP 
);

CREATE TABLE IF NOT EXISTS profile (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    lastname VARCHAR(50),
    identification VARCHAR(20) UNIQUE,
    gender_id INT,
    slogan TEXT,
    status_id INT,
    createDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    profession_id INT,
    total_likes INT DEFAULT 0,
    CONSTRAINT gender_id_FK FOREIGN KEY (gender_id) REFERENCES gender(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT status_id_FK FOREIGN KEY (status_id) REFERENCES status(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT profession_id_FK FOREIGN KEY (profession_id) REFERENCES profession(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS user (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) UNIQUE,
    password VARCHAR(50),
    bonus_likes INT DEFAULT 0,
    profile_id INT,
    birthdate DATE,
    CONSTRAINT profile_id_FK FOREIGN KEY (profile_id) REFERENCES profile(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS application_users(
    user_id INT,
    application_id INT,
    CONSTRAINT user_application_FK FOREIGN KEY (user_id) REFERENCES user(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT application_user_FK FOREIGN KEY (application_id) REFERENCES application(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS administrator (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(50),
    lastname VARCHAR(50),
    identification VARCHAR(20),
    username VARCHAR(50),
    password VARCHAR(50),
    application_id INT,
    CONSTRAINT administrator_id_FK FOREIGN KEY (application_id) REFERENCES application(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS userlikes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    liked_profile_id INT NOT NULL,
    like_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_match BOOLEAN DEFAULT FALSE,
    CONSTRAINT user_likes_FK FOREIGN KEY (user_id) REFERENCES user(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT profile_likes_FK FOREIGN KEY (liked_profile_id) REFERENCES profile(id) ON DELETE CASCADE ON UPDATE CASCADE,
    UNIQUE KEY unique_like (user_id, liked_profile_id)
);

CREATE TABLE IF NOT EXISTS userdislike (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    disliked_profile_id INT NOT NULL,
    dislike_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT user_dislikes_FK FOREIGN KEY (user_id) REFERENCES user(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT profile_dislikes_FK FOREIGN KEY (disliked_profile_id) REFERENCES profile(id) ON DELETE CASCADE ON UPDATE CASCADE,
    UNIQUE KEY unique_like (user_id, disliked_profile_id)
);

CREATE TABLE IF NOT EXISTS interestProfile (
    profile_id INT,
    interest_id INT,
    PRIMARY KEY (profile_id, interest_id),
    CONSTRAINT profile_interest_FK FOREIGN KEY (profile_id) REFERENCES profile(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT interest_id_FK FOREIGN KEY (interest_id) REFERENCES interest(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS user_match (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user1_id INT,
    user2_id INT,
    match_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT user1_id_FK FOREIGN KEY (user1_id) REFERENCES user(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT user2_id_FK FOREIGN KEY (user2_id) REFERENCES user(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS daily_likes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    date DATE,
    profile_id INT,
    number_likes INT CHECK (number_likes <= 10),
    status BOOLEAN DEFAULT TRUE,
    CONSTRAINT profile_daily_likes_FK FOREIGN KEY (profile_id) REFERENCES profile(id) ON DELETE CASCADE ON UPDATE CASCADE
); 

