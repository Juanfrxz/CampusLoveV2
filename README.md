# 💞 CampusLove

¡Bienvenido a **CampusLove**!  
Una plataforma de citas universitaria desarrollada en C# y .NET 8, diseñada para conectar estudiantes y crear nuevas historias de amor en el campus.  
¡Descubre, interactúa y encuentra tu match ideal! 🎓💘

---

## 🚀 Características principales

- **Registro y autenticación de usuarios** 👤🔐  
  Crea tu perfil, inicia sesión y personaliza tu experiencia.

- **Gestión de perfiles** 📝  
  Visualiza, edita y actualiza tu información personal y preferencias.

- **Sistema de matches y reacciones** 💞  
  Interactúa con otros perfiles, da likes y encuentra coincidencias.

- **Panel de administración** 🛠️  
  Administra usuarios, géneros, intereses, profesiones y estados desde un menú exclusivo para administradores.

- **Interfaz moderna en consola** 🎨  
  Menús interactivos y visuales usando [Spectre.Console](https://spectreconsole.net/) y [Figgle](https://github.com/thephoe/figgle) para una experiencia atractiva y amigable.

- **Soporte para múltiples entidades**  
  - Géneros ⚧️
  - Intereses 🎯
  - Profesiones 💼
  - Estados de relación 💍

- **Mensajes claros y amigables** ✨  
  Confirmaciones, advertencias y errores siempre visibles y comprensibles.

---

## 🛠️ Tecnologías utilizadas

- **C# / .NET 8**
- **MySQL** (persistencia de datos)
- **Dapper** (ORM ligero)
- **Spectre.Console** (UI moderna en consola)
- **Figgle** (Arte ASCII para títulos)

---

## Diagramas
- [Diagrama ER](https://www.mermaidchart.com/raw/00fbe82a-ede9-4354-9372-93d747f6d7ed?theme=light&version=v0.1&format=svg)
- [Diagrama Físico](./db/Diagrama.png)
- [Clases](https://www.mermaidchart.com/raw/7b8cdee8-a5e4-4c3f-bf70-7e5d7352f903?theme=light&version=v0.1&format=svg)

## 📦 Instalación y ejecución

1. Clona el repositorio:
   ```bash
   git clone https://github.com/LauraVargas22/CampusLove.git
   ```
2. Restaura los paquetes y compila el proyecto:
   ```bash
   dotnet build
   ```
3. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```

---

## 🗄️ Base de datos y datos de ejemplo

A continuación se muestra la estructura básica de la base de datos y algunos inserts para poblarla rápidamente:

```sql
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

```

```sql
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

INSERT INTO application (name, description) VALUES ('CampusLove', 'Aplicación que permite el emparejamiento de usuarios.');

INSERT INTO administrator (name, lastname, identification, username, password, application_id) VALUES ('Laura', 'Vargas', '123456789', 'lau22', '123', 1), ('Juan', 'Rodriguez', '987654321', 'Juanxx', '456', 1);
```
---

## 👥 Autores

- Juan David [@Juanfrxz](https://github.com/Juanfrxz)
- Laura Vargas [@LauraVargas22](https://github.com/LauraVargas22)
- Colaboradores: ¡Tu nombre puede aparecer aquí! 🚀

---

## 🤝 Contribuciones

¡Las contribuciones son bienvenidas!  
Si tienes ideas, mejoras o encuentras algún bug, no dudes en abrir un issue o un pull request.

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT.

---

¡Gracias por usar **CampusLove**!  
Donde las historias de amor universitario comienzan... 💞🎓

        