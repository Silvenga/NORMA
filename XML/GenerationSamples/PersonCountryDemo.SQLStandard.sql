﻿START TRANSACTION ISOLATION LEVEL SERIALIZABLE, READ WRITE;

CREATE SCHEMA PersonCountryDemo DEFAULT CHARACTER SET UTF8;

SET SCHEMA 'PERSONCOUNTRYDEMO';

CREATE DOMAIN PersonCountryDemo.Title AS CHARACTER VARYING(4) CONSTRAINT ValueTypeValueConstraint1 CHECK (VALUE IN ('Dr', 'Prof', 'Mr', 'Mrs', 'Miss', 'Ms'));

CREATE TABLE PersonCountryDemo.Person
(
	Person_id INTEGER GENERATED ALWAYS AS IDENTITY(START WITH 1 INCREMENT BY 1) NOT NULL,
	LastName CHARACTER VARYING(30) NOT NULL,
	FirstName CHARACTER VARYING(30) NOT NULL,
	Title PersonCountryDemo.Title,
	Country_name CHARACTER VARYING(20),
	CONSTRAINT InternalUniquenessConstraint1 PRIMARY KEY(Person_id)
);

CREATE TABLE PersonCountryDemo.Country
(
	Country_name CHARACTER VARYING(20) NOT NULL,
	Region_code CHARACTER(8),
	CONSTRAINT InternalUniquenessConstraint3 PRIMARY KEY(Country_name)
);

ALTER TABLE PersonCountryDemo.Person ADD CONSTRAINT Person_FK FOREIGN KEY (Country_name) REFERENCES PersonCountryDemo.Country (Country_name) ON DELETE RESTRICT ON UPDATE RESTRICT;

COMMIT WORK;

