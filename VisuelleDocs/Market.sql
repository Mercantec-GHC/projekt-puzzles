CREATE TABLE "Users" (
  "UserId" INT PRIMARY KEY NOT NULL,
  "Username" varchar(225) UNIQUE NOT NULL,
  "Email" varchar(225) NOT NULL,
  "PassHash" varchar(225) NOT NULL,
  "PhoneNumber" varchar(20) NOT NULL,
  "CreatedAt" TIMESTAMP NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE "Advert" (
  "AdvertId" INT PRIMARY KEY NOT NULL,
  "Title" varchar(225) NOT NULL,
  "Description" varchar(225) NOT NULL,
  "UserId" INT NOT NULL,
  "Price" FLOAT NOT NULL,
  "PieceAmount" INT NOT NULL,
  "BoxDimHeight" FLOAT,
  "BoxDimWidth" FLOAT,
  "BoxDimDepth" FLOAT,
  "PuzzleDimHeight" FLOAT,
  "PuzzleDimWidth" FLOAT,
  "Picture" BYTEA,
  "IsSold" BOOLEAN NOT NULL,
  "CreatedAt" TIMESTAMP NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

ALTER TABLE "Advert" ADD FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId");
