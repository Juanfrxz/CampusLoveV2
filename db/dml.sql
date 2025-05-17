
-- Insertar profesiones
INSERT INTO profession (description) VALUES 
('Systems Engineering'),
('Medicine'),
('Psychology'),
('Law'),
('Architecture'),
('Accounting'),
('Marketing'),
('Nursing'),
('Graphic Design'),
('Administration');

-- Insertar géneros
INSERT INTO gender (description) VALUES 
('Male'),
('Female'),
('No binary');

-- Insertar estados
INSERT INTO status (description) VALUES 
('Single'),
('Open Relationship'),
('Married');

-- Insertar intereses
INSERT INTO interest (description) VALUES 
('Music'),
('Films'),
('Travel'),
('Sports'),
('Literature'),
('Photography'),
('Cook'),
('Art'),
('Technology'),
('Nature');


--Insertar aplicación
INSERT INTO application (name, description) VALUES ('CampusLove', 'Aplicación que permite el emparejamiento de usuarios.');

-- Insertar perfiles
INSERT INTO profile (name, lastname, identification, gender_id, slogan, status_id, profession_id, total_likes) VALUES 
('María', 'Rodríguez', '1098765432', 2, 'La vida es una aventura', 1, 1, 0),
('Juan', 'Martínez', '1087654321', 1, 'Amante de la naturaleza', 1, 2, 0),
('Laura', 'Díaz', '1076543210', 2, 'Explorando nuevos horizontes', 2, 3, 0),
('Carlos', 'Gómez', '1065432109', 1, 'Aprendiendo cada día', 1, 4, 0),
('Ana', 'Sánchez', '1054321098', 2, 'Viviendo el presente', 3, 5, 0),
('Pedro', 'López', '1043210987', 1, 'Siempre positivo', 1, 6, 0),
('Sofía', 'García', '1032109876', 2, 'Creativa y apasionada', 2, 7, 0),
('Diego', 'Hernández', '1021098765', 1, 'Aventurero por naturaleza', 1, 8, 0),
('Valentina', 'Torres', '1010987654', 2, 'Arte y diseño', 3, 9, 0),
('Andrés', 'Ramírez', '1009876543', 1, 'Liderando el cambio', 1, 10, 0),
('Camila', 'Morales', '1008765432', 2, 'Innovación constante', 2, 1, 0),
('Daniel', 'Castro', '1007654321', 1, 'Construyendo futuro', 1, 2, 0);

-- Insertar usuarios
INSERT INTO `user` (username, password, profile_id, birthdate) VALUES 
('maria_r', 'password123', 1, '1995-05-10'),
('juan_m', 'securepass456', 2, '1997-08-22'),
('laura_d', 'mypass789', 3, '1990-12-15'),
('carlos_g', 'pass123', 4, '1993-03-18'),
('ana_s', 'secure789', 5, '1996-07-25'),
('pedro_l', 'pass456', 6, '1994-11-30'),
('sofia_g', 'secure123', 7, '1998-04-12'),
('diego_h', 'pass789', 8, '1992-09-05'),
('valentina_t', 'secure456', 9, '1997-01-20'),
('andres_r', 'pass321', 10, '1995-06-15'),
('camila_m', 'secure789', 11, '1993-08-28'),
('daniel_c', 'pass654', 12, '1996-02-14');

-- Insertar likes
INSERT INTO userlikes (user_id, liked_profile_id, like_date, is_match) VALUES 
(1, 2, CURRENT_TIMESTAMP, FALSE),
(2, 1, CURRENT_TIMESTAMP, FALSE),
(3, 1, CURRENT_TIMESTAMP, FALSE),
(4, 3, CURRENT_TIMESTAMP, FALSE),
(5, 2, CURRENT_TIMESTAMP, FALSE),
(6, 7, CURRENT_TIMESTAMP, FALSE),
(7, 6, CURRENT_TIMESTAMP, FALSE),
(8, 9, CURRENT_TIMESTAMP, FALSE),
(9, 8, CURRENT_TIMESTAMP, FALSE),
(10, 11, CURRENT_TIMESTAMP, FALSE),
(11, 10, CURRENT_TIMESTAMP, FALSE),
(12, 5, CURRENT_TIMESTAMP, FALSE);

-- Insertar intereses de perfiles
INSERT INTO interestProfile (profile_id, interest_id) VALUES 
(1, 1), (1, 2), (1, 3),
(2, 2), (2, 3), (2, 4),
(3, 1), (3, 3), (3, 5),
(4, 2), (4, 4), (4, 5),
(5, 1), (5, 2), (5, 3),
(6, 4), (6, 5), (6, 6),
(7, 7), (7, 8), (7, 9),
(8, 1), (8, 3), (8, 10),
(9, 2), (9, 8), (9, 9),
(10, 5), (10, 7), (10, 10),
(11, 1), (11, 9), (11, 10),
(12, 2), (12, 4), (12, 6);

-- Insertar matches
INSERT INTO user_match (user1_id, user2_id, match_date) VALUES 
(1, 2, CURRENT_TIMESTAMP),
(3, 4, CURRENT_TIMESTAMP),
(5, 6, CURRENT_TIMESTAMP),
(7, 8, CURRENT_TIMESTAMP),
(9, 10, CURRENT_TIMESTAMP),
(11, 12, CURRENT_TIMESTAMP);

-- Insertar likes diarios
INSERT INTO daily_likes (date, profile_id, number_likes, status) VALUES 
(CURDATE(), 1, 5, TRUE),
(CURDATE(), 2, 3, TRUE),
(CURDATE(), 3, 7, TRUE),
(CURDATE(), 4, 2, TRUE),
(CURDATE(), 5, 4, TRUE),
(CURDATE(), 6, 6, TRUE),
(CURDATE(), 7, 3, TRUE),
(CURDATE(), 8, 5, TRUE),
(CURDATE(), 9, 4, TRUE),
(CURDATE(), 10, 7, TRUE),
(CURDATE(), 11, 2, TRUE),
(CURDATE(), 12, 5, TRUE);

--Insertar administrador
INSERT INTO administrator (name, lastname, identification, username, password, application_id) VALUES ('Laura', 'Vargas', '123456789', 'lau22', '123', 1), ('Juan', 'Rodriguez', '987654321', 'Juanxx', '456', 1);