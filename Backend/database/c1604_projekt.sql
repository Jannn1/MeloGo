-- phpMyAdmin SQL Dump
-- version 4.9.10
-- https://www.phpmyadmin.net/
--
-- Gép: localhost
-- Létrehozás ideje: 2025. Ápr 27. 19:33
-- Kiszolgáló verziója: 10.5.28-MariaDB-0+deb11u1
-- PHP verzió: 7.4.33

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `c1604_projekt`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Ertekeles`
--

CREATE TABLE `Ertekeles` (
  `ert_id` int(11) NOT NULL,
  `Erdatum` datetime DEFAULT current_timestamp(),
  `Comment` text DEFAULT NULL,
  `Ertekeles` tinyint(4) DEFAULT NULL CHECK (`Ertekeles` between 1 and 5),
  `Ertekelo_Id` int(11) NOT NULL,
  `Ertekelt_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `Ertekeles`
--

INSERT INTO `Ertekeles` (`ert_id`, `Erdatum`, `Comment`, `Ertekeles`, `Ertekelo_Id`, `Ertekelt_Id`) VALUES
(1, '2025-04-23 14:51:42', 'Nagyon jó munka volt!', 5, 1, 2),
(2, '2025-04-23 14:51:42', 'Korrekt és pontos munkavégzés.', 4, 2, 1),
(3, '2025-04-23 14:51:42', 'Gyors, megbízható!', 5, 3, 1),
(4, '2025-04-23 14:51:42', 'Elégedett vagyok az eredménnyel.', 4, 4, 2),
(5, '2025-04-23 14:51:42', 'Rugalmas és segítőkész volt.', 5, 5, 2),
(6, '2025-04-23 14:51:42', 'Kiváló munka, pontos és gyors.', 5, 6, 3),
(7, '2025-04-23 14:51:42', 'Takaros és precíz, elégedett vagyok.', 4, 7, 3),
(8, '2025-04-23 14:51:42', 'Egy kis késés volt, de összességében jó.', 3, 8, 4),
(9, '2025-04-23 14:51:42', 'Nagyszerű munka, csak ajánlani tudom.', 5, 9, 5),
(10, '2025-04-23 14:51:42', 'Kicsit drága, de minőségi szolgáltatás.', 4, 10, 6),
(11, '2025-04-27 18:28:00', 'Jó munkaerő', 5, 1, 4),
(12, '2025-04-27 18:52:00', 'Jól dolgozik', 3, 1, 5);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Feladat`
--

CREATE TABLE `Feladat` (
  `task_id` int(11) NOT NULL,
  `Statusz` enum('nyitott','lezárt') DEFAULT 'nyitott',
  `Helyszin` varchar(255) DEFAULT NULL,
  `Cim` varchar(255) NOT NULL,
  `Posztdatum` datetime DEFAULT current_timestamp(),
  `Hatarido` date DEFAULT NULL,
  `Leiras` text DEFAULT NULL,
  `User_id` int(11) DEFAULT NULL,
  `Idotartam` varchar(255) NOT NULL,
  `Fizetes` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `Feladat`
--

INSERT INTO `Feladat` (`task_id`, `Statusz`, `Helyszin`, `Cim`, `Posztdatum`, `Hatarido`, `Leiras`, `User_id`, `Idotartam`, `Fizetes`) VALUES
(1, 'lezárt', 'Budapest', 'Kert rendezés', '2025-04-08 17:28:10', '2025-04-15', 'Levágni a füvet és gereblyézni', 1, '3 óra', 8000),
(2, 'nyitott', 'Debrecen', 'Takarítás albérletben', '2025-04-08 17:28:10', '2025-04-12', '2 szobás lakás takarítása', 2, '2 óra', 10000),
(3, 'lezárt', 'Szeged', 'Költöztetés', '2025-04-08 17:28:10', '2025-04-10', 'Néhány doboz átszállítása', 3, '1 óra', 5000),
(4, 'nyitott', 'Pécs', 'Laptop javítás', '2025-04-08 17:28:10', '2025-04-20', 'Nem kapcsol be a gép', 4, '1.5 óra', 15000),
(5, 'nyitott', 'Győr', 'Fordítás németről', '2025-04-08 17:28:10', '2025-04-22', '1 oldal szöveg fordítása', 5, '0.5 óra', 4000),
(6, 'lezárt', 'Miskolc', 'Autó beindítása', '2025-04-08 17:28:10', '2025-04-18', 'Lemerült az akku', 6, '30 perc', 6000),
(7, 'nyitott', 'Budapest', 'Angol korrepetálás', '2025-04-08 17:28:10', '2025-04-25', 'Középfokú szinten', 7, '1 óra', 7000),
(8, 'nyitott', 'Veszprém', 'Futárszolgálat', '2025-04-08 17:28:10', '2025-04-14', 'Csomag házhoz szállítása', 8, '2 óra', 9000),
(9, 'nyitott', 'Sopron', 'Takarítás háznál', '2025-04-08 17:28:10', '2025-04-13', 'Terasz és ablakok tisztítása', 9, '2.5 óra', 9500),
(10, 'lezárt', 'Szolnok', 'Lámpacsere', '2025-04-08 17:28:10', '2025-04-05', 'Kiégett a konyhai világítás', 1, '1 óra', 5000),
(17, 'nyitott', 'Szombathely', 'Pakolás', '2025-04-27 19:11:00', '2025-10-09', 'Garázspakolás', 1, '2 óra', 10000),
(18, 'nyitott', 'Szombathely', 'Fűnyírás', '2025-04-27 19:26:00', '2025-10-09', '2000m2 kell lenyírni', 1, '2 óra', 10000);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `FeladatKategoria`
--

CREATE TABLE `FeladatKategoria` (
  `Task_id` int(11) NOT NULL,
  `Kat_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `FeladatKategoria`
--

INSERT INTO `FeladatKategoria` (`Task_id`, `Kat_id`) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 7),
(5, 9),
(6, 10),
(7, 6),
(8, 4),
(9, 2),
(10, 8);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Felhasznalo`
--

CREATE TABLE `Felhasznalo` (
  `user_id` int(11) NOT NULL,
  `Jelszo` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Szuldat` date DEFAULT NULL,
  `Veznev` varchar(100) DEFAULT NULL,
  `Kernev` varchar(100) DEFAULT NULL,
  `Profilkep` varchar(255) DEFAULT NULL,
  `Bio` text DEFAULT NULL,
  `Regdatum` datetime DEFAULT current_timestamp(),
  `Felhtipus` enum('admin','user') DEFAULT 'user',
  `Telefonszam` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `Felhasznalo`
--

INSERT INTO `Felhasznalo` (`user_id`, `Jelszo`, `Email`, `Szuldat`, `Veznev`, `Kernev`, `Profilkep`, `Bio`, `Regdatum`, `Felhtipus`, `Telefonszam`) VALUES
(1, '$2a$11$Ee0lAA6.TEivRjh8nP6sxuDbx5CK8mzahChLGCrFniAiNhtb67gkK', 'felhasznalo1@test.com', '1990-05-10', 'Kovács', 'Anna', 'uploads/resized_image_680ceb6c05c350.00805148.png', 'Szeretem az állatokat2', '2024-10-16 19:17:45', 'user', '+36 30/123-4567'),
(2, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo2@test.com', '1985-07-20', 'Szabó', 'Béla', NULL, 'Barkácsolok', '2024-10-20 11:02:25', 'user', '+36 20/234-5678'),
(3, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo3@test.com', '1992-03-14', 'Nagy', 'Cili', NULL, 'Kutyasétáltatás a hobbim', '2025-01-11 18:44:12', 'user', '+36 70/345-6789'),
(4, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo4@test.com', '1994-11-02', 'Kiss', 'Dani', NULL, 'Futárként dolgozom', '2024-11-08 18:22:20', 'user', '+36 30/456-7890'),
(5, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo5@test.com', '1988-08-08', 'Varga', 'Emese', NULL, NULL, '2025-04-07 07:07:40', 'user', '+36 20/567-8901'),
(6, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo6@test.com', '1991-01-19', 'Tóth', 'Feri', NULL, 'Szeretem a technikát', '2025-01-19 23:05:15', 'user', '+36 70/678-9012'),
(7, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo7@test.com', '1987-12-12', 'Pintér', 'Géza', NULL, NULL, '2024-12-15 20:10:38', 'user', '+36 30/789-0123'),
(8, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo8@test.com', '1993-06-06', 'Lakatos', 'Hajni', NULL, 'Rendszeres önkéntes', '2025-03-09 10:21:31', 'user', '+36 20/890-1234'),
(9, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo9@test.com', '1986-09-09', 'Molnár', 'István', NULL, NULL, '2025-03-22 03:30:07', 'user', '+36 70/901-2345'),
(10, '871b32b5f4e1b9ac25237dc7e4e175954c2dc6098aade48a8abefb585cbd53f2', 'felhasznalo10@test.com', '1980-01-01', 'Nagy', 'Elemér', NULL, 'Rendszergazda', '2024-11-27 06:33:55', 'admin', '+36 30/012-3456'),
(16, '$2a$11$Ee0lAA6.TEivRjh8nP6sxuDbx5CK8mzahChLGCrFniAiNhtb67gkK', 'asd@asd.com', NULL, NULL, NULL, NULL, NULL, '2025-04-25 13:28:00', 'user', NULL),
(17, '$2a$11$MNiHzEs0ShlGkexz2w5/betAOpfe59Sx7oE3KDFlFXplK.WDESPga', 'vikczall2005@gmail.com', '1999-02-02', NULL, NULL, NULL, NULL, '2025-04-25 15:45:00', 'user', NULL),
(18, '$2a$11$ZS9Q3fu7AOALrJi5W3YJ..td5s9F7lOzKDt27U0MZuN2DKPlwYlOe', 'vendeg1@test.com', NULL, NULL, NULL, NULL, NULL, '2025-04-26 20:00:00', 'user', NULL);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Jelentkezesek`
--

CREATE TABLE `Jelentkezesek` (
  `Statusz` enum('függőben','elfogadva','elutasítva') DEFAULT 'függőben',
  `Latta_e` tinyint(1) DEFAULT 0,
  `Jeldatum` datetime DEFAULT current_timestamp(),
  `User_id` int(11) NOT NULL,
  `Task_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `Jelentkezesek`
--

INSERT INTO `Jelentkezesek` (`Statusz`, `Latta_e`, `Jeldatum`, `User_id`, `Task_id`) VALUES
('függőben', 0, '2025-04-25 13:19:00', 1, 2),
('függőben', 0, '2025-04-25 12:58:00', 1, 7),
('függőben', 1, '2025-04-08 17:28:53', 2, 1),
('elfogadva', 1, '2025-04-08 17:28:53', 2, 10),
('elfogadva', 1, '2025-04-08 17:28:53', 3, 2),
('elutasítva', 1, '2025-04-08 17:28:53', 4, 3),
('elfogadva', 1, '2025-04-08 17:28:53', 5, 4),
('függőben', 0, '2025-04-08 17:28:53', 6, 5),
('függőben', 0, '2025-04-08 17:28:53', 7, 6),
('elutasítva', 1, '2025-04-08 17:28:53', 8, 7),
('elfogadva', 1, '2025-04-08 17:28:53', 9, 8),
('függőben', 0, '2025-04-08 17:28:53', 10, 9),
('elutasítva', 0, '2025-04-27 09:53:00', 18, 1),
('függőben', 0, '2025-04-27 09:36:00', 18, 5);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Kategoria`
--

CREATE TABLE `Kategoria` (
  `kat_id` int(11) NOT NULL,
  `Katnev` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `Kategoria`
--

INSERT INTO `Kategoria` (`kat_id`, `Katnev`) VALUES
(1, 'Kertészkedés'),
(2, 'Takarítás'),
(3, 'Költöztetés'),
(4, 'Futár'),
(5, 'Kutyasétáltatás'),
(6, 'Tanítás'),
(7, 'Számítástechnika'),
(8, 'Szerelés'),
(9, 'Fordítás'),
(10, 'Autós segítség');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `Mentes`
--

CREATE TABLE `Mentes` (
  `User_id` int(11) DEFAULT NULL,
  `Task_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

--
-- A tábla adatainak kiíratása `Mentes`
--

INSERT INTO `Mentes` (`User_id`, `Task_id`) VALUES
(1, 2),
(2, 3),
(3, 4),
(4, 5),
(5, 6),
(6, 7),
(7, 8),
(8, 9),
(9, 10),
(10, 1),
(1, 1),
(18, 5);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `Ertekeles`
--
ALTER TABLE `Ertekeles`
  ADD PRIMARY KEY (`ert_id`),
  ADD KEY `ertekelo_id` (`Ertekelo_Id`),
  ADD KEY `ertekelt_id` (`Ertekelt_Id`);

--
-- A tábla indexei `Feladat`
--
ALTER TABLE `Feladat`
  ADD PRIMARY KEY (`task_id`),
  ADD KEY `user_id` (`User_id`);

--
-- A tábla indexei `FeladatKategoria`
--
ALTER TABLE `FeladatKategoria`
  ADD PRIMARY KEY (`Task_id`,`Kat_id`),
  ADD KEY `kat_id` (`Kat_id`);

--
-- A tábla indexei `Felhasznalo`
--
ALTER TABLE `Felhasznalo`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `email` (`Email`);

--
-- A tábla indexei `Jelentkezesek`
--
ALTER TABLE `Jelentkezesek`
  ADD PRIMARY KEY (`User_id`,`Task_id`),
  ADD KEY `User_id` (`User_id`),
  ADD KEY `Jelentkezesek_ibfk_2` (`Task_id`);

--
-- A tábla indexei `Kategoria`
--
ALTER TABLE `Kategoria`
  ADD PRIMARY KEY (`kat_id`);

--
-- A tábla indexei `Mentes`
--
ALTER TABLE `Mentes`
  ADD KEY `user_id` (`User_id`),
  ADD KEY `task_id` (`Task_id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `Ertekeles`
--
ALTER TABLE `Ertekeles`
  MODIFY `ert_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT a táblához `Feladat`
--
ALTER TABLE `Feladat`
  MODIFY `task_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT a táblához `Felhasznalo`
--
ALTER TABLE `Felhasznalo`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT a táblához `Kategoria`
--
ALTER TABLE `Kategoria`
  MODIFY `kat_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `Ertekeles`
--
ALTER TABLE `Ertekeles`
  ADD CONSTRAINT `ertekeles_ibfk_1` FOREIGN KEY (`Ertekelo_Id`) REFERENCES `Felhasznalo` (`user_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `ertekeles_ibfk_2` FOREIGN KEY (`Ertekelt_Id`) REFERENCES `Felhasznalo` (`user_id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `Feladat`
--
ALTER TABLE `Feladat`
  ADD CONSTRAINT `Feladat_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Felhasznalo` (`user_id`);

--
-- Megkötések a táblához `FeladatKategoria`
--
ALTER TABLE `FeladatKategoria`
  ADD CONSTRAINT `FeladatKategoria_ibfk_1` FOREIGN KEY (`Task_id`) REFERENCES `Feladat` (`task_id`),
  ADD CONSTRAINT `FeladatKategoria_ibfk_2` FOREIGN KEY (`Kat_id`) REFERENCES `Kategoria` (`kat_id`);

--
-- Megkötések a táblához `Jelentkezesek`
--
ALTER TABLE `Jelentkezesek`
  ADD CONSTRAINT `Jelentkezesek_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Felhasznalo` (`user_id`),
  ADD CONSTRAINT `Jelentkezesek_ibfk_2` FOREIGN KEY (`Task_id`) REFERENCES `Feladat` (`task_id`);

--
-- Megkötések a táblához `Mentes`
--
ALTER TABLE `Mentes`
  ADD CONSTRAINT `Mentes_ibfk_1` FOREIGN KEY (`User_id`) REFERENCES `Felhasznalo` (`user_id`),
  ADD CONSTRAINT `Mentes_ibfk_2` FOREIGN KEY (`Task_id`) REFERENCES `Feladat` (`task_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
