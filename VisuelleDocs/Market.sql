CREATE TABLE "Users" (
  "UserId" INT PRIMARY KEY NOT NULL,
  "Name" VARCHAR(225) NOT NULL,
  "Email" VARCHAR(225) NOT NULL,
  "PassHash" VARCHAR(225) NOT NULL,
  "PhoneNumber" VARCHAR(20) NOT NULL,
  "CreatedAt" TIMESTAMP NOT NULL
);

CREATE TABLE "Advert" (
  "AdvertId" INT PRIMARY KEY NOT NULL,
  "Name" VARCHAR(225) NOT NULL,
  "Description" VARCHAR(225) NOT NULL,
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
  "CreatedAt" TIMESTAMP NOT NULL
);

ALTER TABLE "Advert" ADD FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId");
