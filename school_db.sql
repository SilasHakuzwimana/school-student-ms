-- MariaDB dump 10.19  Distrib 10.4.32-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: school_db
-- ------------------------------------------------------
-- Server version	10.4.32-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `students` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) DEFAULT NULL,
  `email` varchar(100) NOT NULL,
  `tel` varchar(20) NOT NULL,
  `age` int(11) DEFAULT NULL,
  `reg_number` int(9) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  UNIQUE KEY `reg_number` (`reg_number`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `tel` (`tel`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` VALUES (1,'Silas HAKUZWIMANA','hakuzwisilas@gmail.com','+250 783 749 019',21,223001019,'2025-05-13 11:36:09'),(2,'Gentille TUMUKUNDE','tumukunde@gmail.com','+250 784 974 645',28,223007846,'2025-05-13 11:37:11'),(3,'Gentille TUMUKUNDE','gentille.t@gmail.com','+250 789 111 222',22,223001020,'2025-05-13 12:07:26'),(4,'Eric NDAYISHIMIYE','eric.ndayi@example.com','+250 782 333 444',23,223001021,'2025-05-13 12:07:26'),(5,'Aline UWASE','aline.uwase@example.com','+250 788 555 666',20,223001022,'2025-05-13 12:07:26'),(6,'Yves MUGISHA','yves.mugisha@example.com','+250 783 777 888',24,223001023,'2025-05-13 12:07:26'),(7,'Chantal KABANDA','kabanda.chantal@gmail.com','+250 780 999 000',21,223001024,'2025-05-13 12:07:26'),(8,'Patrick UWIMANA','patrick.u@example.com','+250 781 321 654',22,223001025,'2025-05-13 12:07:26'),(9,'Diane MUKANDORI','diane.mukandori@gmail.com','+250 787 432 123',23,223001026,'2025-05-13 12:07:26'),(10,'Jean Paul KAGABO','jpkagabo@example.com','+250 789 876 543',25,223001027,'2025-05-13 12:07:26'),(11,'Nadia NYIRAHABIMANA','nadia2@gmail.com','+250 786 234 567',19,223001028,'2025-05-13 12:07:26'),(12,'Bosco KANYANDEKWE','kanyabosc23@gmail.com','+250 784 655 324',34,223001919,'2025-05-13 12:29:29'),(13,'Alice Uwimana','alice.u@example.com','+250783456789',21,222040001,'2025-05-13 12:55:45'),(14,'Jean Bosco Nshimiyimana','jb.nshimiyimana@example.com','+250788123456',23,222040002,'2025-05-13 12:55:45'),(15,'Clarisse Mukamana','clarisse.m@example.com','+250781234567',22,222040003,'2025-05-13 12:55:45'),(16,'Eric Habimana','eric.h@example.com','+250789654321',24,222040004,'2025-05-13 12:55:45'),(17,'Sandrine Ingabire','sandrine.i@example.com','+250782345678',20,222040005,'2025-05-13 12:55:45'),(18,'Patrick Mugisha','patrick.m@example.com','+250787987654',25,222040006,'2025-05-13 12:55:45'),(19,'Diane Umutoni','diane.u@example.com','+250784321987',22,222040007,'2025-05-13 12:55:45'),(20,'Kevin Ishimwe','kevin.i@example.com','+250785678123',23,222040008,'2025-05-13 12:55:45');
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-13 13:07:09
