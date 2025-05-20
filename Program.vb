Imports System
Imports System.IO
Imports MySql.Data.MySqlClient
Imports System.Text.RegularExpressions
Imports System.Data
Imports System.Threading

Module Program
    ' 🔑 Database connection string
    Dim connStr As String = "server=localhost;user=root;password=;database=school_db;"

    ' UI Constants
    Private Const HEADER_COLOR As ConsoleColor = ConsoleColor.Cyan
    Private Const SUCCESS_COLOR As ConsoleColor = ConsoleColor.Green
    Private Const ERROR_COLOR As ConsoleColor = ConsoleColor.Red
    Private Const WARNING_COLOR As ConsoleColor = ConsoleColor.Yellow
    Private Const INPUT_COLOR As ConsoleColor = ConsoleColor.White
    Private Const INFO_COLOR As ConsoleColor = ConsoleColor.Gray
    Private Const TITLE_COLOR As ConsoleColor = ConsoleColor.Magenta

    ' UI Elements
    Private Const DOUBLE_LINE As String = "═══════════════════════════════════════"
    Private Const SINGLE_LINE As String = "───────────────────────────────────────-───────────────────────────────────────-─────────────────---"

    ' ╔══════════════════ Helper Functions ══════════════════╗

    Sub DisplayHeader(title As String)
        Console.Clear()
        Console.ForegroundColor = TITLE_COLOR
        Console.WriteLine(DOUBLE_LINE)
        Console.WriteLine($"  {title}  ")
        Console.WriteLine(DOUBLE_LINE)
        Console.ResetColor()
    End Sub

    Sub DisplayFooter()
        Console.ForegroundColor = INFO_COLOR
        Console.WriteLine(SINGLE_LINE)
        Console.ResetColor()
    End Sub

    Sub DisplaySuccess(message As String)
        Console.ForegroundColor = SUCCESS_COLOR
        Console.WriteLine($"✅ {message}")
        Console.ResetColor()
    End Sub

    Sub DisplayError(message As String)
        Console.ForegroundColor = ERROR_COLOR
        Console.WriteLine($"❌ {message}")
        Console.ResetColor()
    End Sub

    Sub DisplayWarning(message As String)
        Console.ForegroundColor = WARNING_COLOR
        Console.WriteLine($"⚠️ {message}")
        Console.ResetColor()
    End Sub

    Sub DisplayInfo(message As String)
        Console.ForegroundColor = INFO_COLOR
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Sub PressAnyKey()
        ' Clear the current line completely
        Console.Write($"{vbCr}{New String(" "c, Console.WindowWidth - 1)}{vbCr}")

        ' Then write the message
        DisplayInfo("Press any key to continue...")
        Console.ReadKey(True) ' True prevents the key from appearing on the screen
    End Sub

    Function GetValidInput(prompt As String, Optional isRequired As Boolean = True) As String
        Console.ForegroundColor = INPUT_COLOR
        Console.Write(prompt)
        Console.ResetColor()
        Dim input As String = Console.ReadLine().Trim()

        If isRequired AndAlso String.IsNullOrWhiteSpace(input) Then
            DisplayError("This field cannot be empty!")
            Return GetValidInput(prompt, isRequired)
        End If

        Return input
    End Function

    Function GetValidEmail(prompt As String) As String
        Dim email As String = GetValidInput(prompt)
        Dim pattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"

        If Not Regex.IsMatch(email, pattern) Then
            DisplayError("Invalid email format!")
            Return GetValidEmail(prompt)
        End If

        Return email
    End Function

    Function GetValidPhone(prompt As String) As String
        Dim phone As String = GetValidInput(prompt)
        ' Allow + prefix, digits, and spaces in phone numbers
        Dim pattern As String = "^\+?[\d\s]{10,20}$"

        If Not Regex.IsMatch(phone, pattern) Then
            DisplayError("Invalid phone number! (Format example: +250 783 456 465)")
            Return GetValidPhone(prompt)
        End If

        ' Check that the number has at least 10 digits (ignoring spaces)
        Dim digitsOnly As String = Regex.Replace(phone, "\D", "")
        If digitsOnly.Length < 10 OrElse digitsOnly.Length > 15 Then
            DisplayError("Phone number must have between 10-15 digits!")
            Return GetValidPhone(prompt)
        End If

        Return phone
    End Function

    Function GetValidInteger(prompt As String, Optional min As Integer = Integer.MinValue, Optional max As Integer = Integer.MaxValue) As Integer
        Try
            Console.ForegroundColor = INPUT_COLOR
            Console.Write(prompt)
            Console.ResetColor()
            Dim input As String = Console.ReadLine().Trim()

            If String.IsNullOrWhiteSpace(input) Then
                Throw New ArgumentException("Input cannot be empty")
            End If

            Dim value As Integer = Convert.ToInt32(input)

            If value < min OrElse value > max Then
                Throw New ArgumentOutOfRangeException($"Value must be between {min} and {max}")
            End If

            Return value
        Catch ex As FormatException
            DisplayError("Please enter a valid number!")
            Return GetValidInteger(prompt, min, max)
        Catch ex As ArgumentOutOfRangeException
            DisplayError(ex.ParamName)
            Return GetValidInteger(prompt, min, max)
        Catch ex As Exception
            DisplayError(ex.Message)
            Return GetValidInteger(prompt, min, max)
        End Try
    End Function

    Function IsStudentIdValid(id As Integer) As Boolean
        Try
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim sql As String = "SELECT COUNT(*) FROM students WHERE id = @id"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", id)
                    Dim count = Convert.ToInt32(cmd.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            DisplayError($"Database error: {ex.Message}")
            Return False
        End Try
    End Function

    Sub AnimateLoading(message As String, duration As Integer)
        Dim frames() As String = {"|", "/", "-", "\"}
        Dim startTime As DateTime = DateTime.Now
        Dim frameIndex As Integer = 0

        ' Calculate total characters to overwrite (for clean erasing)
        Dim cleanLength As Integer = message.Length + 2 ' +2 for space + frame

        While (DateTime.Now - startTime).TotalMilliseconds < duration
            Console.Write($"{vbCr}{message} {frames(frameIndex)}")
            Thread.Sleep(100)
            frameIndex = (frameIndex + 1) Mod frames.Length
        End While

        ' Clean up the line after animation
        Console.Write($"{vbCr}{New String(" "c, cleanLength)}{vbCr}")
    End Sub

    ' ╔══════════════════ Database Functions ══════════════════╗

    Function TryConnect() As Boolean
        Try
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Return True
            End Using
        Catch ex As Exception
            DisplayError($"Cannot connect to database: {ex.Message}")
            Return False
        End Try
    End Function

    ' ╔══════════════════ Main Functions ══════════════════╗

    Sub InsertStudent()
        DisplayHeader("📝 INSERT NEW STUDENT")

        Try
            ' Input validation
            Dim name As String = GetValidInput("👤 Name: ")
            Dim email As String = GetValidEmail("📧 Email: ")
            Dim tel As String = GetValidPhone("📱 Phone Number: ")
            Dim age As Integer = GetValidInteger("🎂 Age: ", 5, 120)
            Dim reg As String = GetValidInput("🆔 Registration Number: ")

            DisplayInfo("Validating information...")
            Thread.Sleep(500)  ' Add a brief pause for visual effect

            AnimateLoading("Processing", 1000)

            Using conn As New MySqlConnection(connStr)
                conn.Open()

                ' Check if registration number already exists
                Dim checkSql As String = "SELECT COUNT(*) FROM students WHERE reg_number = @reg"
                Using checkCmd As New MySqlCommand(checkSql, conn)
                    checkCmd.Parameters.AddWithValue("@reg", reg)
                    Dim count = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If count > 0 Then
                        DisplayError("Registration number already exists!")
                        PressAnyKey()
                        Return
                    End If
                End Using

                ' Insert the student
                Dim sql As String = "INSERT INTO students (name, email, tel, age, reg_number) VALUES (@name, @email, @tel, @age, @reg)"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@name", name)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@tel", tel)
                    cmd.Parameters.AddWithValue("@age", age)
                    cmd.Parameters.AddWithValue("@reg", reg)
                    cmd.ExecuteNonQuery()
                    DisplaySuccess("Student inserted successfully.")
                End Using
            End Using
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try

        PressAnyKey()
    End Sub

    Sub ViewStudents()
        DisplayHeader("📋 VIEW ALL STUDENTS")

        Try
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim sql As String = "SELECT * FROM students ORDER BY id ASC"
                Using cmd As New MySqlCommand(sql, conn)
                    Using reader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Console.ForegroundColor = HEADER_COLOR
                            Console.WriteLine("{0,-5} {1,-18} {2,-25} {3,-19} {4,-3} {5,-9} {6}",
                                "ID", "Name", "Email", "Phone", "Age", "Reg. No", "Created At")
                            Console.WriteLine(SINGLE_LINE)
                            Console.ResetColor()

                            Dim rowCount As Integer = 0
                            While reader.Read()
                                rowCount += 1
                                ' Alternate row colors for better readability
                                If rowCount Mod 2 = 0 Then
                                    Console.ForegroundColor = INFO_COLOR
                                End If

                                Console.WriteLine("{0,-5} {1,-18} {2,-25} {3,-19} {4,-5} {5,-9} {6}",
                                    reader("id"), reader("name"), reader("email"),
                                    reader("tel"), reader("age"), reader("reg_number"), reader("created_at").ToShortDateString())

                                Console.ResetColor()
                            End While

                            DisplayInfo($"{vbCrLf}Total records: {rowCount}")
                        Else
                            DisplayWarning("No students found in the database.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try

        PressAnyKey()
    End Sub

    Sub UpdateStudent()
        DisplayHeader("✏️ UPDATE STUDENT")

        Try
            Dim id As Integer = GetValidInteger("Enter Student ID to update: ")

            If Not IsStudentIdValid(id) Then
                DisplayError("Student ID not found!")
                PressAnyKey()
                Return
            End If

            ' Display current student data
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim getSql As String = "SELECT * FROM students WHERE id = @id"
                Using getCmd As New MySqlCommand(getSql, conn)
                    getCmd.Parameters.AddWithValue("@id", id)
                    Using reader = getCmd.ExecuteReader()
                        If reader.Read() Then
                            DisplayInfo($"{vbCrLf}Current Student Details:")
                            Console.ForegroundColor = HEADER_COLOR
                            Console.WriteLine("{0,-10}: {1}", "Name", reader("name"))
                            Console.WriteLine("{0,-10}: {1}", "Email", reader("email"))
                            Console.WriteLine("{0,-10}: {1}", "Phone", reader("tel"))
                            Console.WriteLine("{0,-10}: {1}", "Age", reader("age"))
                            Console.WriteLine("{0,-10}: {1}", "Reg. No", reader("reg_number"))
                            Console.ResetColor()
                            Console.WriteLine()
                        End If
                    End Using
                End Using
            End Using

            DisplayInfo("Enter new details (leave blank to keep current value):")

            ' Get updated info
            Dim name As String = GetValidInput("New Name: ", False)
            Dim email As String = GetValidInput("New Email: ", False)
            If Not String.IsNullOrWhiteSpace(email) AndAlso Not Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") Then
                email = GetValidEmail("New Email (valid format): ")
            End If

            Dim tel As String = GetValidInput("New Phone: ", False)
            If Not String.IsNullOrWhiteSpace(tel) AndAlso Not Regex.IsMatch(tel, "^\+?[0-9]{10,15}$") Then
                tel = GetValidPhone("New Phone (valid format): ")
            End If

            Dim ageStr As String = GetValidInput("New Age: ", False)
            Dim age As Integer? = Nothing
            If Not String.IsNullOrWhiteSpace(ageStr) Then
                Try
                    age = Convert.ToInt32(ageStr)
                    If age < 5 OrElse age > 120 Then
                        age = GetValidInteger("New Age (between 5-120): ", 5, 120)
                    End If
                Catch ex As FormatException
                    age = GetValidInteger("New Age (numeric): ", 5, 120)
                End Try
            End If

            Dim reg As String = GetValidInput("New Registration Number: ", False)

            ' Process update
            AnimateLoading("Updating", 800)

            Using conn As New MySqlConnection(connStr)
                conn.Open()

                ' Start a transaction
                Dim transaction As MySqlTransaction = conn.BeginTransaction()

                Try
                    ' Build dynamic SQL for update
                    Dim updateParts As New List(Of String)
                    Dim sql As String = "UPDATE students SET "
                    Dim cmd As New MySqlCommand("", conn, transaction)

                    If Not String.IsNullOrWhiteSpace(name) Then
                        updateParts.Add("name=@name")
                        cmd.Parameters.AddWithValue("@name", name)
                    End If

                    If Not String.IsNullOrWhiteSpace(email) Then
                        updateParts.Add("email=@email")
                        cmd.Parameters.AddWithValue("@email", email)
                    End If

                    If Not String.IsNullOrWhiteSpace(tel) Then
                        updateParts.Add("tel=@tel")
                        cmd.Parameters.AddWithValue("@tel", tel)
                    End If

                    If age.HasValue Then
                        updateParts.Add("age=@age")
                        cmd.Parameters.AddWithValue("@age", age.Value)
                    End If

                    If Not String.IsNullOrWhiteSpace(reg) Then
                        ' Check if reg number exists
                        Dim checkSql As String = "SELECT COUNT(*) FROM students WHERE reg_number = @reg AND id <> @id"
                        Using checkCmd As New MySqlCommand(checkSql, conn, transaction)
                            checkCmd.Parameters.AddWithValue("@reg", reg)
                            checkCmd.Parameters.AddWithValue("@id", id)
                            Dim count = Convert.ToInt32(checkCmd.ExecuteScalar())
                            If count > 0 Then
                                transaction.Rollback()
                                DisplayError("Registration number already exists for another student!")
                                PressAnyKey()
                                Return
                            End If
                        End Using

                        updateParts.Add("reg_number=@reg")
                        cmd.Parameters.AddWithValue("@reg", reg)
                    End If

                    ' Check if there's anything to update
                    If updateParts.Count = 0 Then
                        DisplayWarning("No changes were made.")
                        transaction.Rollback()
                        PressAnyKey()
                        Return
                    End If

                    sql &= String.Join(", ", updateParts) & " WHERE id=@id"
                    cmd.CommandText = sql
                    cmd.Parameters.AddWithValue("@id", id)

                    Dim rowsAffected = cmd.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        transaction.Commit()
                        DisplaySuccess("Student updated successfully.")
                    Else
                        transaction.Rollback()
                        DisplayError("Update failed.")
                    End If
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try

        PressAnyKey()
    End Sub

    Sub DeleteStudent()
        DisplayHeader("🗑️ DELETE STUDENT")

        Try
            Dim id As Integer = GetValidInteger("Enter Student ID to delete: ")

            If Not IsStudentIdValid(id) Then
                DisplayError("Student ID not found!")
                PressAnyKey()
                Return
            End If

            ' Display student info before deletion
            Dim studentName As String = ""
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim getSql As String = "SELECT name, reg_number FROM students WHERE id = @id"
                Using getCmd As New MySqlCommand(getSql, conn)
                    getCmd.Parameters.AddWithValue("@id", id)
                    Using reader = getCmd.ExecuteReader()
                        If reader.Read() Then
                            studentName = reader("name").ToString()
                            DisplayInfo($"{vbCrLf}You are about to delete:")
                            Console.ForegroundColor = WARNING_COLOR
                            Console.WriteLine($"Student: {studentName} (ID: {id}, Reg: {reader("reg_number")})")
                            Console.ResetColor()
                        End If
                    End Using
                End Using
            End Using

            ' Confirm deletion
            Console.ForegroundColor = ERROR_COLOR
            Console.Write(vbCrLf & "⚠️ WARNING: This action cannot be undone! Continue? (Y/N): ")
            Console.ResetColor()
            Dim confirm As String = Console.ReadLine().Trim().ToUpper()

            If confirm <> "Y" Then
                DisplayInfo("Deletion cancelled.")
                PressAnyKey()
                Return
            End If

            AnimateLoading("Deleting", 800)

            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim sql As String = "DELETE FROM students WHERE id=@id"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", id)
                    Dim rowsAffected = cmd.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        DisplaySuccess($"Student '{studentName}' deleted successfully.")
                    Else
                        DisplayError("Delete operation failed.")
                    End If
                End Using
            End Using
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try

        PressAnyKey()
    End Sub

    Sub ExportToCSV()
        DisplayHeader("📤 EXPORT STUDENTS TO CSV")

        Try
            Dim filePath As String = $"students_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"

            AnimateLoading("Preparing export", 800)

            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Dim sql As String = "SELECT * FROM students ORDER BY id ASC"
                Using cmd As New MySqlCommand(sql, conn)
                    Using reader = cmd.ExecuteReader()
                        ' Check if there's data to export
                        If Not reader.HasRows Then
                            DisplayWarning("No student data to export.")
                            PressAnyKey()
                            Return
                        End If

                        Using writer As New StreamWriter(filePath)
                            ' Write CSV header with quotes for better compatibility
                            writer.WriteLine("""ID"",""Name"",""Email"",""Phone"",""Age"",""RegNumber"",""CreatedAt""")

                            ' Write data rows
                            While reader.Read()
                                ' Properly escape and quote CSV values to handle commas within data
                                Dim id = reader("id").ToString()
                                Dim name = EscapeCsvValue(reader("name").ToString())
                                Dim email = EscapeCsvValue(reader("email").ToString())
                                Dim tel = EscapeCsvValue(reader("tel").ToString())
                                Dim age = reader("age").ToString()
                                Dim regNumber = EscapeCsvValue(reader("reg_number").ToString())
                                Dim createdAt = EscapeCsvValue(reader("created_at").ToString())

                                writer.WriteLine($"{id},{name},{email},{tel},{age},{regNumber},{createdAt}")
                            End While
                        End Using
                    End Using
                End Using
            End Using

            ' Verify file was created
            If File.Exists(filePath) Then
                Dim fileInfo As New FileInfo(filePath)
                DisplaySuccess($"Export successful: {filePath}")
                DisplayInfo($"File size: {fileInfo.Length / 1024:N2} KB")

                ' Ask if user wants to open the file
                Console.Write(vbCrLf & "Would you like to open the file now? (Y/N): ")
                Dim openFile As String = Console.ReadLine().Trim().ToUpper()

                If openFile = "Y" Then
                    Try
                        Process.Start("notepad.exe", filePath)
                    Catch ex As Exception
                        DisplayError($"Could not open file: {ex.Message}")
                    End Try
                End If
            Else
                DisplayError("Failed to create export file.")
            End If
        Catch ex As IOException
            DisplayError($"File error: {ex.Message}")
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try

        PressAnyKey()
    End Sub

    Private Function EscapeCsvValue(value As String) As String
        ' If value contains quotes or commas, wrap in quotes and escape internal quotes
        If value.Contains("""") OrElse value.Contains(",") Then
            Return """" & value.Replace("""", """""") & """"
        Else
            Return """" & value & """"
        End If
    End Function

    Sub ViewStudentsWithPagination()
        Dim pageSize As Integer = 5
        Dim page As Integer = 0
        Dim totalRows As Integer = 0
        Dim totalPages As Integer = 0

        Try
            Using conn As New MySqlConnection(connStr)
                conn.Open()
                ' Count total rows
                Using countCmd As New MySqlCommand("SELECT COUNT(*) FROM students", conn)
                    totalRows = Convert.ToInt32(countCmd.ExecuteScalar())
                    totalPages = Math.Ceiling(totalRows / pageSize)

                    If totalRows = 0 Then
                        DisplayHeader("📄 PAGINATED STUDENT LIST")
                        DisplayWarning("No students found in the database.")
                        PressAnyKey()
                        Return
                    End If
                End Using

                Do
                    DisplayHeader("📄 PAGINATED STUDENT LIST")
                    Console.ForegroundColor = INFO_COLOR
                    Console.WriteLine($"Page {page + 1} of {totalPages} (Total Records: {totalRows})")
                    Console.ResetColor()

                    Dim offset As Integer = page * pageSize
                    Dim sql As String = "SELECT * FROM students ORDER BY id ASC LIMIT @limit OFFSET @offset"
                    Using cmd As New MySqlCommand(sql, conn)
                        cmd.Parameters.AddWithValue("@limit", pageSize)
                        cmd.Parameters.AddWithValue("@offset", offset)

                        Using reader = cmd.ExecuteReader()
                            Console.ForegroundColor = HEADER_COLOR
                            Console.WriteLine("{0,-5} {1,-18} {2,-25} {3,-15} {4,-5} {5,-14} {6}",
                                "ID", "Name", "Email", "Phone", "Age", "Reg. No", "Created At")
                            Console.WriteLine(SINGLE_LINE)
                            Console.ResetColor()

                            Dim rowCount As Integer = 0
                            While reader.Read()
                                rowCount += 1
                                ' Alternate row colors for better readability
                                If rowCount Mod 2 = 0 Then
                                    Console.ForegroundColor = INFO_COLOR
                                End If

                                Console.WriteLine("{0,-5} {1,-18} {2,-25} {3,-15} {4,-5} {5,-14} {6}",
                                    reader("id"), reader("name"), reader("email"),
                                    reader("tel"), reader("age"), reader("reg_number"), reader("created_at").ToShortDateString()
                                    )

                                Console.ResetColor()
                            End While
                        End Using
                    End Using

                    ' Navigation options
                    Console.WriteLine(vbCrLf & "Navigation Options:")
                    Console.ForegroundColor = HEADER_COLOR
                    Console.WriteLine("[N] Next     [P] Previous     [F] First     [L] Last     [G] Go to Page     [Q] Quit")
                    Console.ResetColor()

                    Dim nav = Console.ReadKey(True).Key

                    Select Case nav
                        Case ConsoleKey.N ' Next Page
                            If (page + 1) < totalPages Then
                                page += 1
                            Else
                                DisplayWarning("Already on the last page!")
                                Thread.Sleep(800)
                            End If
                        Case ConsoleKey.P ' Previous Page
                            If page > 0 Then
                                page -= 1
                            Else
                                DisplayWarning("Already on the first page!")
                                Thread.Sleep(800)
                            End If
                        Case ConsoleKey.F ' First Page
                            page = 0
                        Case ConsoleKey.L ' Last Page
                            page = totalPages - 1
                        Case ConsoleKey.G ' Go to specific page
                            Console.Write(vbCrLf & "Enter page number (1-" & totalPages & "): ")
                            Dim pageInput As String = Console.ReadLine().Trim()
                            Dim pageNum As Integer

                            If Integer.TryParse(pageInput, pageNum) AndAlso pageNum >= 1 AndAlso pageNum <= totalPages Then
                                page = pageNum - 1
                            Else
                                DisplayError("Invalid page number!")
                                Thread.Sleep(800)
                            End If
                        Case ConsoleKey.Q ' Quit
                            Exit Do
                    End Select
                Loop
            End Using
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try
    End Sub

    Sub SearchStudents()
        DisplayHeader("🔍 SEARCH STUDENTS")

        Try
            ' Get search parameters
            DisplayInfo("Search by:")
            Console.WriteLine("1. Name")
            Console.WriteLine("2. Registration Number")
            Console.WriteLine("3. Email")
            Console.WriteLine("4. Age Range")
            Console.Write(vbCrLf & "Select search option (1-4): ")

            Dim choice As String = Console.ReadLine().Trim()

            Dim sql As String = ""
            Dim parameters As New Dictionary(Of String, Object)
            Dim searchTitle As String = ""

            Select Case choice
                Case "1" ' Search by name
                    Dim name As String = GetValidInput("Enter student name: ")
                    sql = "SELECT * FROM students WHERE name LIKE @keyword ORDER BY id ASC"
                    parameters.Add("@keyword", "%" & name & "%")
                    searchTitle = $"Search results for name: '{name}'"

                Case "2" ' Search by registration number
                    Dim regNumber As String = GetValidInput("Enter registration number: ")
                    sql = "SELECT * FROM students WHERE reg_number LIKE @keyword ORDER BY id ASC"
                    parameters.Add("@keyword", "%" & regNumber & "%")
                    searchTitle = $"Search results for registration number: '{regNumber}'"

                Case "3" ' Search by email
                    Dim email As String = GetValidInput("Enter email: ")
                    sql = "SELECT * FROM students WHERE email LIKE @keyword ORDER BY id ASC"
                    parameters.Add("@keyword", "%" & email & "%")
                    searchTitle = $"Search results for email: '{email}'"

                Case "4" ' Search by age range
                    Dim minAge As Integer = GetValidInteger("Enter minimum age: ", 0, 120)
                    Dim maxAge As Integer = GetValidInteger("Enter maximum age: ", minAge, 120)
                    sql = "SELECT * FROM students WHERE age BETWEEN @minAge AND @maxAge ORDER BY id ASC"
                    parameters.Add("@minAge", minAge)
                    parameters.Add("@maxAge", maxAge)
                    searchTitle = $"Search results for age range: {minAge} to {maxAge}"

                Case Else
                    DisplayError("Invalid choice!")
                    PressAnyKey()
                    Return
            End Select

            AnimateLoading("Searching", 800)

            DisplayInfo(vbCrLf & searchTitle)
            DisplayFooter()

            Using conn As New MySqlConnection(connStr)
                conn.Open()
                Using cmd As New MySqlCommand(sql, conn)
                    ' Add all parameters
                    For Each param In parameters
                        cmd.Parameters.AddWithValue(param.Key, param.Value)
                    Next

                    Using reader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            Console.ForegroundColor = HEADER_COLOR
                            Console.WriteLine("{0,-5} {1,-18} {2,-25} {3,-19} {4,-5} {5,-9} {6}",
                                "ID", "Name", "Email", "Phone", "Age", "Reg. No", "Created At")
                            Console.WriteLine(SINGLE_LINE)
                            Console.ResetColor()

                            Dim rowCount As Integer = 0
                            While reader.Read()
                                rowCount += 1
                                ' Alternate row colors for better readability
                                If rowCount Mod 2 = 0 Then
                                    Console.ForegroundColor = INFO_COLOR
                                End If

                                Console.WriteLine("{0,-5} {1,-18} {2,-25} {3,-19} {4,-5} {5,-9} {6}",
                                    reader("id"), reader("name"), reader("email"),
                                    reader("tel"), reader("age"), reader("reg_number"), reader("created_at").ToShortDateString())

                                Console.ResetColor()
                            End While

                            DisplayInfo($"{vbCrLf}Total records found: {rowCount}")
                        Else
                            DisplayWarning("No matching records found.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            DisplayError($"Database error: {ex.Message}")
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
        End Try

        PressAnyKey()
    End Sub

    Sub AboutApplication()
        DisplayHeader("ℹ️ ABOUT APPLICATION")

        Console.ForegroundColor = TITLE_COLOR
        Console.WriteLine("STUDENT MANAGEMENT SYSTEM")
        Console.WriteLine("Version 1.0.0")
        Console.ResetColor()

        Console.WriteLine(vbCrLf & "This application manages student records with the following features:")
        Console.WriteLine("• Add new students")
        Console.WriteLine("• View all students")
        Console.WriteLine("• Update student information")
        Console.WriteLine("• Delete students")
        Console.WriteLine("• Export student data to CSV")
        Console.WriteLine("• Search for students")
        Console.WriteLine("• Paginated view of student records")

        Console.WriteLine(vbCrLf & "Database Information:")
        Try
            Using conn As New MySqlConnection(connStr)
                conn.Open()

                ' Get MySQL version
                Dim versionCmd As New MySqlCommand("SELECT VERSION()", conn)
                Dim version As String = versionCmd.ExecuteScalar().ToString()

                ' Get student count
                Dim countCmd As New MySqlCommand("SELECT COUNT(*) FROM students", conn)
                Dim count As Integer = Convert.ToInt32(countCmd.ExecuteScalar())

                Console.WriteLine($"• MySQL Version: {version}")
                Console.WriteLine($"• Database: school_db")
                Console.WriteLine($"• Table: students")
                Console.WriteLine($"• Total Students: {count}")

                DisplaySuccess($"{vbCrLf}Database connection successful")
            End Using
        Catch ex As Exception
            DisplayError($"{vbCrLf}Database connection failed: {ex.Message}")
        End Try

        Console.WriteLine(vbCrLf & "© 2025 Student Management System. All rights reserved.")

        PressAnyKey()
    End Sub

    Sub InitializeDatabase()
        Try
            DisplayInfo("Checking database connection...")

            Using conn As New MySqlConnection(connStr)
                conn.Open()
                DisplayInfo("Connection successful.")

                ' Check if students table exists
                Dim tableExists As Boolean = False
                Dim checkTableSql As String = "SHOW TABLES LIKE 'students'"

                Using cmd As New MySqlCommand(checkTableSql, conn)
                    Using reader = cmd.ExecuteReader()
                        tableExists = reader.HasRows
                    End Using
                End Using

                ' Create table if it doesn't exist
                If Not tableExists Then
                    DisplayInfo("Creating students table...")

                    Dim createTableSql As String = "
                        CREATE TABLE students (
                            id INT AUTO_INCREMENT PRIMARY KEY,
                            name VARCHAR(100) NOT NULL,
                            email VARCHAR(100) NOT NULL,
                            tel VARCHAR(20) NOT NULL,
                            age INT NOT NULL,
                            reg_number VARCHAR(50) NOT NULL,
                            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                        )"
                Else
                    DisplayInfo("Students table already exists.")
                End If
            End Using
        Catch ex As MySqlException
            DisplayError($"Database initialization error: {ex.Message}")

            If ex.Message.Contains("Unknown database") Then
                DisplayInfo("Please create the 'school_db' database manually or update the connection string.")
            End If

            PressAnyKey()
            Environment.Exit(1)
        Catch ex As Exception
            DisplayError($"Error: {ex.Message}")
            PressAnyKey()
            Environment.Exit(1)
        End Try
    End Sub

    Sub Main()
        ' Set console properties
        Console.Title = "Student Management System"
        Console.SetWindowSize(100, 50)
        Console.OutputEncoding = System.Text.Encoding.UTF8

        Try
            ' Display welcome splash screen
            Console.Clear()
            Console.ForegroundColor = TITLE_COLOR
            Console.WriteLine(DOUBLE_LINE)
            Console.WriteLine("
    ██████╗████████╗██╗   ██╗██████╗ ███████╗███╗   ██╗████████╗
    ██╔════╝╚══██╔══╝██║   ██║██╔══██╗██╔════╝████╗  ██║╚══██╔══╝
    ███████╗   ██║   ██║   ██║██║  ██║█████╗  ██╔██╗ ██║   ██║   
    ╚════██║   ██║   ██║   ██║██║  ██║██╔══╝  ██║╚██╗██║   ██║   
    ███████║   ██║   ╚██████╔╝██████╔╝███████╗██║ ╚████║   ██║   
    ╚══════╝   ╚═╝    ╚═════╝ ╚═════╝ ╚══════╝╚═╝  ╚═══╝   ╚═╝   
                                                            
    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ 
    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗
    ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝
    ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗
    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║
    ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝
            ")
            Console.WriteLine(DOUBLE_LINE)
            Console.ResetColor()

            Console.WriteLine(vbCrLf & "     Welcome to Student Management System")
            Console.WriteLine("     Loading application...")

            ' Animation for loading
            AnimateLoading("     Initializing", 1500)

            ' Initialize database
            InitializeDatabase()

            ' If we get here, database is ready
            Dim choice As Integer

            Do
                Console.Clear()
                Console.SetWindowSize(100, 30)
                DisplayHeader("🎓 STUDENT MANAGEMENT SYSTEM 🎓")

                Console.WriteLine("Menu Options:")
                Console.WriteLine("1. Insert Student")
                Console.WriteLine("2. View All Students")
                Console.WriteLine("3. View Students (Paginated)")
                Console.WriteLine("4. Update Student")
                Console.WriteLine("5. Delete Student")
                Console.WriteLine("6. Search Students")
                Console.WriteLine("7. Export Students to CSV")
                Console.WriteLine("8. About Application")
                Console.WriteLine("0. Exit")

                Console.Write(vbCrLf & "Enter your choice (0-8): ")

                If Not Integer.TryParse(Console.ReadLine(), choice) Then
                    DisplayError("Invalid option! Please enter a number.")
                    Thread.Sleep(1000)
                    Continue Do
                End If

                Select Case choice
                    Case 1
                        InsertStudent()
                    Case 2
                        ViewStudents()
                    Case 3
                        ViewStudentsWithPagination()
                    Case 4
                        UpdateStudent()
                    Case 5
                        DeleteStudent()
                    Case 6
                        SearchStudents()
                    Case 7
                        ExportToCSV()
                    Case 8
                        AboutApplication()
                    Case 0
                        DisplayHeader("👋 GOODBYE!")
                        DisplayInfo("Thank you for using Student Management System.")
                        Thread.Sleep(1500)
                        Exit Do
                    Case Else
                        DisplayError("Invalid option! Please try again.")
                        Thread.Sleep(1000)
                End Select
            Loop
        Catch ex As Exception
            DisplayError($"Unhandled exception: {ex.Message}")
            DisplayInfo("Application will now exit.")
            PressAnyKey()
        End Try
    End Sub
End Module