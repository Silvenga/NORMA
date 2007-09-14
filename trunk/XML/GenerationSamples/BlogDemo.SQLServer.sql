﻿CREATE SCHEMA BlogDemo
GO

GO

CREATE TABLE BlogDemo.BlogEntry
(
	BlogEntry_Id INTEGER NOT NULL,
	EntryTitle NATIONAL CHARACTER VARYING(30) NOT NULL,
	EntryBody CHARACTER LARGE OBJECT() NOT NULL,
	PostedDate TIMESTAMP NOT NULL,
	UserIdName2 NATIONAL CHARACTER VARYING(30) NOT NULL,
	UserIdName1 NATIONAL CHARACTER VARYING(30) NOT NULL,
	BlogCommentParentEntryId INTEGER,
	CONSTRAINT InternalUniquenessConstraint1 PRIMARY KEY(BlogEntry_Id)
)
GO


CREATE TABLE BlogDemo."User"
(
	Name1 NATIONAL CHARACTER VARYING(30) NOT NULL,
	Name2 NATIONAL CHARACTER VARYING(30) NOT NULL,
	Username NATIONAL CHARACTER VARYING(30) NOT NULL,
	Password NATIONAL CHARACTER(32) NOT NULL,
	CONSTRAINT ExternalUniquenessConstraint1 PRIMARY KEY(Name1, Name2)
)
GO


CREATE TABLE BlogDemo.BlogLabel
(
	BlogLabel_Id INTEGER IDENTITY (1, 1) NOT NULL,
	Title CHARACTER LARGE OBJECT(),
	CONSTRAINT InternalUniquenessConstraint18 PRIMARY KEY(BlogLabel_Id)
)
GO


CREATE TABLE BlogDemo.BlogEntryLabel
(
	BlogEntryId INTEGER NOT NULL,
	BlogLabelId INTEGER NOT NULL,
	CONSTRAINT InternalUniquenessConstraint20 PRIMARY KEY(BlogEntryId, BlogLabelId)
)
GO


ALTER TABLE BlogDemo.BlogEntry ADD CONSTRAINT BlogEntry_BlogEntry_FK1 FOREIGN KEY (UserIdName2, UserIdName1) REFERENCES BlogDemo."User" (Name1, Name2) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


ALTER TABLE BlogDemo.BlogEntry ADD CONSTRAINT BlogEntry_BlogEntry_FK2 FOREIGN KEY (BlogCommentParentEntryId) REFERENCES BlogDemo.BlogEntry (BlogEntry_Id) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


ALTER TABLE BlogDemo.BlogEntryLabel ADD CONSTRAINT BlogEntryLabel_BlogEntryLabel_FK1 FOREIGN KEY (BlogEntryId) REFERENCES BlogDemo.BlogEntry (BlogEntry_Id) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


ALTER TABLE BlogDemo.BlogEntryLabel ADD CONSTRAINT BlogEntryLabel_BlogEntryLabel_FK2 FOREIGN KEY (BlogLabelId) REFERENCES BlogDemo.BlogLabel (BlogLabel_Id) ON DELETE NO ACTION ON UPDATE NO ACTION
GO



GO