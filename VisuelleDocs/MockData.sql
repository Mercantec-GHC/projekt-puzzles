INSERT INTO "UserAccounts" (
  "Username",
  "Email",
  "PassHash",
  "PhoneNumber"
)
SELECT
  'user' || s AS "Username",
  'user' || s || '@example.com' AS "Email",
  'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=' AS "PassHash",
  -- Generates a 10-digit number for phone number
  '555-' || LPAD(FLOOR(random() * 1000000000)::text, 10, '0') AS "PhoneNumber"
FROM generate_series(1, 10) AS s;

INSERT INTO "Advert" (
  "Title",
  "Description",
  "UserId",
  "Price",
  "PieceAmount",
  "BoxDimHeight",
  "BoxDimWidth",
  "BoxDimDepth",
  "PuzzleDimHeight",
  "PuzzleDimWidth",
  "IsSold"
)
SELECT
  'Puzzle ' || s AS "Title",
  -- Generates a random 10-character alphanumeric string for description
  'A random puzzle ad ' || MD5(random()::text) AS "Description",
  -- Assigns a random UserId between 1 and 10
  FLOOR(random() * 10) + 1 AS "UserId",
  -- Generates a price between 10.00 and 100.99
  ROUND((10.00 + (random() * 90.99))::numeric, 2) AS "Price",
  -- Generates a piece amount between 500 and 3000
  FLOOR(random() * 2501) + 500 AS "PieceAmount",
  -- Generates box dimensions between 10.0 and 40.0
  ROUND((10.0 + (random() * 30.0))::numeric, 1) AS "BoxDimHeight",
  ROUND((10.0 + (random() * 30.0))::numeric, 1) AS "BoxDimWidth",
  ROUND((5.0 + (random() * 15.0))::numeric, 1) AS "BoxDimDepth",
  -- Generates puzzle dimensions between 30.0 and 80.0
  ROUND((30.0 + (random() * 50.0))::numeric, 1) AS "PuzzleDimHeight",
  ROUND((30.0 + (random() * 50.0))::numeric, 1) AS "PuzzleDimWidth",
  -- Generates a random boolean (TRUE/FALSE)
  (random() < 0.5) AS "IsSold"
FROM generate_series(1, 200) AS s;