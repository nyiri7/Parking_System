CREATE TABLE User (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL 
);

CREATE TABLE Car (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    license_plate TEXT NOT NULL,
    model TEXT,
    brand TEXT
);

CREATE TABLE ParkingSpotType (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT NOT NULL
);

CREATE TABLE ParkingSpot (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type_id INTEGER,
    available BOOLEAN DEFAULT 1,
    FOREIGN KEY (type_id) REFERENCES ParkingSpotType(id)
);

CREATE TABLE ParkingReservation (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    from_time DATETIME NOT NULL,
    to_time DATETIME NOT NULL,
    spot_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    car_id INTEGER,
    FOREIGN KEY (spot_id) REFERENCES ParkingSpot(id),
    FOREIGN KEY (user_id) REFERENCES User(id),
    FOREIGN KEY (car_id) REFERENCES Car(id)
);

