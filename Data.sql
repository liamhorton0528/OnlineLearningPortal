BEGIN TRANSACTION;
INSERT INTO Enrollments (CourseId, StudentId, EnrollmentDate) VALUES (1, 1, '2023-09-01');
INSERT INTO Enrollments (CourseId, StudentId, EnrollmentDate) VALUES (2, 2, '2023-09-15');
INSERT INTO Enrollments (CourseId, StudentId, EnrollmentDate) VALUES (3, 3, '2023-10-01');
INSERT INTO Enrollments (CourseId, StudentId, EnrollmentDate) VALUES (4, 4, '2023-10-15');
INSERT INTO Enrollments (CourseId, StudentId, EnrollmentDate) VALUES (5, 5, '2023-11-01');

INSERT INTO Students (Name, Email, BirthDate) VALUES ('John Doe', 'john.doe@example.com', '2000-01-15');
INSERT INTO Students (Name, Email, BirthDate) VALUES ('Jane Smith', 'jane.smith@example.com', '1999-05-23');
INSERT INTO Students (Name, Email, BirthDate) VALUES ('Michael Johnson', 'michael.j@example.com', '2001-07-30');
INSERT INTO Students (Name, Email, BirthDate) VALUES ('Emily Davis', 'emily.d@example.com', '2000-11-12');
INSERT INTO Students (Name, Email, BirthDate) VALUES ('William Brown', 'william.b@example.com', '1998-03-05');

INSERT INTO Courses (Title, Description, InstructorId) VALUES ('Introduction to AI', 'Basics of Artificial Intelligence', 1);
INSERT INTO Courses (Title, Description, InstructorId) VALUES ('Web Development', 'Building modern web applications', 2);
INSERT INTO Courses (Title, Description, InstructorId) VALUES ('Data Science', 'Data analysis and visualization', 3);
INSERT INTO Courses (Title, Description, InstructorId) VALUES ('Machine Learning', 'Advanced machine learning concepts', 4);
INSERT INTO Courses (Title, Description, InstructorId) VALUES ('Cloud Computing', 'Cloud infrastructure and services', 5);

INSERT INTO Instructors (Name, Email, Office, Phone) VALUES ('Dr. Alice Walker', 'alice.walker@example.com', 'Room 101', '555-1234');
INSERT INTO Instructors (Name, Email, Office, Phone) VALUES ('Dr. Bob Martin', 'bob.martin@example.com', 'Room 102', '555-5678');
INSERT INTO Instructors (Name, Email, Office, Phone) VALUES ('Dr. Carol White', 'carol.white@example.com', 'Room 103', '555-8765');
INSERT INTO Instructors (Name, Email, Office, Phone) VALUES ('Dr. David Green', 'david.green@example.com', 'Room 104', '555-4321');
INSERT INTO Instructors (Name, Email, Office, Phone) VALUES ('Dr. Eve Black', 'eve.black@example.com', 'Room 105', '555-6789');

COMMIT TRANSACTION;