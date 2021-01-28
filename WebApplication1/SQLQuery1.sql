USE [s9817]
SELECT * FROM Student2;

CREATE TABLE Student2 (
    IndexNumber nvarchar(100)  NOT NULL,
    FirstName nvarchar(100)  NOT NULL,
    LastName nvarchar(100)  NOT NULL,
    IdEnrollment int  NOT NULL,
    CONSTRAINT Student2_pk PRIMARY KEY  (IndexNumber)
);
INSERT INTO Student2 (IndexNumber, FirstName, LastName, IdEnrollment)
Values ('s9817', 'Mateusz', 'Lajnert', 1);

INSERT INTO Student2 (IndexNumber, FirstName, LastName, IdEnrollment)
Values ('s1234', 'Weronika', 'Wlocka', 2);

INSERT INTO Student2 (IndexNumber, FirstName, LastName, IdEnrollment)
Values ('s12356', 'Konrad', 'Wanowicz', 3);

INSERT INTO Student2 (IndexNumber, FirstName, LastName, IdEnrollment)
Values ('s98761', 'Joanna', 'Kazikowa', 4);

INSERT INTO Student2 (IndexNumber, FirstName, LastName, IdEnrollment)
Values ('s45980', 'Melania', 'Tramp', 5);


ALTER TABLE Student2 
    add Password nvarchar(250);

ALTER TABLE Student2 
    add Salt nvarchar(250);

ALTER TABLE Student2 
    add RefreshToken nvarchar(250);



CREATE TABLE Enrollment (
    IdEnrollment    int  NOT NULL,
    Semester        int  NOT NULL,
    IdStudy         int  NOT NULL,
    StartDate       date NOT NULL,
    CONSTRAINT Enrollment_pk PRIMARY KEY  (IdEnrollment)
);

INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
Values (1, 8, 1, '01-09-2015');
INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
Values (2, 6, 2, '01-09-2016');
INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
Values (3, 4, 3, '01-09-2017');
INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
Values (4, 2, 4, '01-09-2018');
INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)
Values (5, 2, 5, '01-09-2018');

USE s9817
SELECT * FROM Student2;