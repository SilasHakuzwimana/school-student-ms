
# 🎓 Student Management System

**Version:** 1.0.0
**© 2025 Student Management System. All rights reserved.**

---

## ℹ️ About the Application

The **Student Management System** is a **console-based application developed in Visual Basic (VB)**. It provides a user-friendly, text-based interface to manage student records stored in a MySQL database. Designed for simplicity and efficiency, this tool allows seamless CRUD operations and data management through the command line.

---

## ✅ Key Features

- ➕ Add new students
- 👁️ View all student records
- ✏️ Update existing student information
- 🗑️ Delete student entries
- 📤 Export student data to CSV format
- 🔍 Search and filter students
- 📄 Paginated view of student records for improved navigation

---

## 🗄️ Database Information

- **Database Server:** MySQL
- **Version:** 10.4.32-MariaDB
- **Database Name:** `school_db`
- **Primary Table:** `students`
- **Total Records:** 20 students (sample)

---

## ⚙️ Setup Instructions

1. **Clone the Repository**

   ```bash
   git clone https://github.com/SilasHakuzwimana/school-student-ms.git
   ```
2. **Import the Database**

   - Open a MySQL client or phpMyAdmin.
   - Run the following SQL:
     ```sql
     CREATE DATABASE school_db;
     USE school_db;
     -- Import the `students` table structure and sample data
     ```
3. **Configure the VB Application**

   - Open the project in Visual Studio or your preferred VB.NET IDE.
   - Locate the database connection block and update it:
     ```vb
     Dim connectionString As String = "server=localhost;userid=root;password=;database=school_db"
     ```
4. **Run the Application**

   - Start the application from the IDE or compile and execute the binary.
   - Use the interactive menu via the console to perform desired operations.

---

## ✅ Application Status

- 🔌 **Database Connection:** Successful
- 🚀 **Application:** Running and fully functional

---

## 🧰 Technologies Used

- **Language:** Visual Basic (.NET Framework)
- **Database:** MySQL 10.4.32-MariaDB
- **Interface:** Console (CLI)
- **Export Format:** CSV

---

## 📃 License

This project is provided for educational purposes only.
For commercial or production use, please contact the author for licensing.

---

## ✉️ Contact

**Author:** Silas HAKUZWIMANA
**Phone:** +250 783 749 019
**Email:** [hakuzwisilas@gmail.com](mailto:hakuzwisilas@gmail.com)

---
