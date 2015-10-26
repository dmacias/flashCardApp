Italian -> Spanish Flash Card Application
==============================================================
A simple web app to showcase my C# skills to potential employers.

Stack Used
------------------
* Mono
* ASP.NET MVC / Web Api
* C#
* MySql
* Linux

Overview
--------------------
The application is made up of an ASP.NET MVC web app that serves a single static html page (**FlashCardApp**) and a console app that imports a customized format of categorized/prioritized, Italian to Spanish translations (**TranslationImporter**).  There's also a unit testing project that tests a few methods in the application.

If you just want to see the application in action, visit [flashcards.danmacias.com](http://flashcards.danmacias.com).

There are also models/tests I left in the project that I use to sharpen my problem solving skills.  They will have the *Puzzle* within the class.

Setup
---------------------
###Applications
This application was built using mono-develop on Linux.  I'm pretty new to mono, so if you're building this project using other tools, please let me know if there are any nuances in how I configured the application that can ease setup in other environments.  A sample import file is provided in TranslationImporter/import.tf.  The sample is also the very same import file that gets imported by the application, so make a copy of the file to keep a reference.  Database values and import file locations shouldn't be hard-coded, but eventually I will put them in a .settings file or a web.config.

###Database
Database database/schema/user is all hard-coded within the application.  Here's a script that sets it all up for you (given that you have GRANT permissions to a MySql DB):

```mysql
CREATE DATABASE flashcardapp; 

USE flashcardapp; 

CREATE TABLE `translation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `italiano` varchar(255) NOT NULL,
  `espanol` varchar(255) DEFAULT NULL,
  `category` varchar(255) NOT NULL,
  `priority` int(255) DEFAULT NULL,
  `created` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`italiano`,`category`),
  UNIQUE KEY `id_surr` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=280 DEFAULT CHARSET=latin1;

CREATE USER 'flashcardapp'@'localhost' IDENTIFIED BY 'ciaobella';

GRANT ALL ON flashcardapp.* TO 'flashcardapp'@'localhost';
```

Feedback
--------------------
Please provide any feedback on architecture, algorithms or structure.  Feedback is always well appreciated. :)
