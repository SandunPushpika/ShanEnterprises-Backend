CREATE TYPE user_role AS ENUM (
    'ADMIN',
    'CUSTOMER',
    'DRIVER'
);

CREATE TYPE user_status AS ENUM (
    'ACTIVE',
    'INACTIVE',
    'SUSPENDED'
);

CREATE TYPE driver_status AS ENUM (
    'PENDING',
    'APPROVED',
    'REJECTED',
    'DEACTIVATED'
);

CREATE TYPE availability_status AS ENUM (
    'AVAILABLE',
    'UNAVAILABLE',
    'ON_TRIP'
);

CREATE TYPE vehicle_status AS ENUM (
    'AVAILABLE',
    'BOOKED',
    'MAINTENANCE',
    'UNAVAILABLE'
);

CREATE TYPE transmission_type AS ENUM (
    'MANUAL',
    'AUTOMATIC'
);

CREATE TYPE fuel_type AS ENUM (
    'PETROL',
    'DIESEL',
    'HYBRID',
    'ELECTRIC'
);

CREATE TYPE booking_status AS ENUM (
    'PENDING',
    'CONFIRMED',
    'ONGOING',
    'COMPLETED',
    'CANCELLED',
    'REJECTED'
);

CREATE TYPE payment_status AS ENUM (
    'PENDING',
    'COMPLETED',
    'FAILED',
    'REFUNDED'
);

CREATE TYPE payment_method AS ENUM (
    'CARD',
    'BANK_TRANSFER',
    'CASH',
    'ONLINE_PAYMENT'
);

CREATE TYPE notification_type AS ENUM (
    'BOOKING',
    'PAYMENT',
    'SYSTEM',
    'REMINDER'
);

-- USERS TABLE
CREATE TABLE users (
    id SERIAL PRIMARY KEY,

    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    phone_number VARCHAR(20) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,

    role user_role NOT NULL DEFAULT 'CUSTOMER',
    status user_status NOT NULL DEFAULT 'ACTIVE',

    profile_image_url TEXT,
    address TEXT,
    city VARCHAR(100),
    nic_passport_number VARCHAR(50),

    email_verified BOOLEAN DEFAULT FALSE,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- DRIVERS TABLE
CREATE TABLE drivers (
    id SERIAL PRIMARY KEY,

    user_id INTEGER UNIQUE NOT NULL REFERENCES users(id) ON DELETE CASCADE,

    license_number VARCHAR(100) UNIQUE NOT NULL,
    license_expiry_date DATE NOT NULL,

    years_of_experience INTEGER DEFAULT 0,

    driver_status driver_status DEFAULT 'PENDING',
    availability availability_status DEFAULT 'AVAILABLE',

    license_document_url TEXT,

    average_rating DECIMAL(3,2) DEFAULT 0.00,
    completed_rides INTEGER DEFAULT 0,

    approved_by INTEGER REFERENCES users(id),
    approved_at TIMESTAMP,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- VEHICLE BRANDS
CREATE TABLE vehicle_brands (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- VEHICLE TYPES
CREATE TABLE vehicle_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- VEHICLES TABLE
CREATE TABLE vehicles (
    id SERIAL PRIMARY KEY,

    brand_id INTEGER REFERENCES vehicle_brands(id),
    type_id INTEGER REFERENCES vehicle_types(id),

    model VARCHAR(150) NOT NULL,
    registration_number VARCHAR(50) UNIQUE NOT NULL,

    manufacture_year INTEGER NOT NULL,
    color VARCHAR(50),

    transmission transmission_type,
    fuel fuel_type,

    seat_capacity INTEGER,
    luggage_capacity INTEGER,

    daily_rental_price DECIMAL(10,2) NOT NULL,
    price_per_km DECIMAL(10,2),

    description TEXT,

    main_image_url TEXT,

    air_conditioned BOOLEAN DEFAULT TRUE,
    has_bluetooth BOOLEAN DEFAULT FALSE,
    has_gps BOOLEAN DEFAULT FALSE,

    status vehicle_status DEFAULT 'AVAILABLE',

    average_rating DECIMAL(3,2) DEFAULT 0.00,
    total_bookings INTEGER DEFAULT 0,

    created_by INTEGER REFERENCES users(id),

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- VEHICLE IMAGES TABLE
CREATE TABLE vehicle_images (
    id SERIAL PRIMARY KEY,

    vehicle_id INTEGER NOT NULL REFERENCES vehicles(id) ON DELETE CASCADE,

    image_url TEXT NOT NULL,
    is_primary BOOLEAN DEFAULT FALSE,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BOOKINGS TABLE
CREATE TABLE bookings (
    id SERIAL PRIMARY KEY,

    customer_id INTEGER NOT NULL REFERENCES users(id),
    vehicle_id INTEGER NOT NULL REFERENCES vehicles(id),
    driver_id INTEGER REFERENCES drivers(id),

    booking_reference VARCHAR(50) UNIQUE NOT NULL,

    pickup_location TEXT NOT NULL,
    dropoff_location TEXT,

    pickup_datetime TIMESTAMP NOT NULL,
    return_datetime TIMESTAMP NOT NULL,

    rental_days INTEGER,
    estimated_distance_km DECIMAL(10,2),

    with_driver BOOLEAN DEFAULT FALSE,

    base_rental_cost DECIMAL(10,2) NOT NULL,
    driver_fee DECIMAL(10,2) DEFAULT 0.00,
    tax_amount DECIMAL(10,2) DEFAULT 0.00,
    discount_amount DECIMAL(10,2) DEFAULT 0.00,
    total_amount DECIMAL(10,2) NOT NULL,

    special_notes TEXT,

    booking_status booking_status DEFAULT 'PENDING',

    cancelled_reason TEXT,
    cancelled_at TIMESTAMP,

    confirmed_at TIMESTAMP,
    completed_at TIMESTAMP,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT valid_booking_dates CHECK (return_datetime > pickup_datetime)
);

-- DRIVER BOOKING REQUESTS
CREATE TABLE driver_booking_requests (
    id SERIAL PRIMARY KEY,

    booking_id INTEGER NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    driver_id INTEGER NOT NULL REFERENCES drivers(id) ON DELETE CASCADE,

    request_status VARCHAR(20) DEFAULT 'PENDING',

    responded_at TIMESTAMP,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- PAYMENTS TABLE
CREATE TABLE payments (
    id SERIAL PRIMARY KEY,

    booking_id INTEGER NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    customer_id INTEGER NOT NULL REFERENCES users(id),

    transaction_reference VARCHAR(255) UNIQUE,

    amount DECIMAL(10,2) NOT NULL,

    payment_method payment_method NOT NULL,
    payment_status payment_status DEFAULT 'PENDING',

    payment_gateway VARCHAR(100),

    paid_at TIMESTAMP,

    failure_reason TEXT,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- REVIEWS TABLE
CREATE TABLE reviews (
    id SERIAL PRIMARY KEY,

    booking_id INTEGER NOT NULL REFERENCES bookings(id) ON DELETE CASCADE,
    customer_id INTEGER NOT NULL REFERENCES users(id),

    vehicle_id INTEGER REFERENCES vehicles(id),
    driver_id INTEGER REFERENCES drivers(id),

    vehicle_rating INTEGER CHECK (vehicle_rating BETWEEN 1 AND 5),
    driver_rating INTEGER CHECK (driver_rating BETWEEN 1 AND 5),

    comment TEXT,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT review_target_check CHECK (
        vehicle_id IS NOT NULL OR driver_id IS NOT NULL
    )
);

-- NOTIFICATIONS TABLE
CREATE TABLE notifications (
    id SERIAL PRIMARY KEY,

    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,

    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,

    notification_type notification_type,

    is_read BOOLEAN DEFAULT FALSE,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- SAVED VEHICLES / FAVORITES
CREATE TABLE favorite_vehicles (
    id SERIAL PRIMARY KEY,

    customer_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    vehicle_id INTEGER NOT NULL REFERENCES vehicles(id) ON DELETE CASCADE,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    UNIQUE(customer_id, vehicle_id)
);

-- RECOMMENDATION LOGS
CREATE TABLE recommendation_logs (
    id SERIAL PRIMARY KEY,

    customer_id INTEGER NOT NULL REFERENCES users(id),
    vehicle_id INTEGER REFERENCES vehicles(id),
    driver_id INTEGER REFERENCES drivers(id),

    recommendation_score DECIMAL(5,2),

    generated_reason TEXT,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- BUDGET ESTIMATIONS
CREATE TABLE budget_estimations (
    id SERIAL PRIMARY KEY,

    customer_id INTEGER REFERENCES users(id),
    vehicle_id INTEGER NOT NULL REFERENCES vehicles(id),

    estimated_days INTEGER,
    estimated_distance_km DECIMAL(10,2),

    include_driver BOOLEAN DEFAULT FALSE,

    estimated_total DECIMAL(10,2) NOT NULL,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE verification_codes (
    id SERIAL PRIMARY KEY,
    
    user_id INTEGER REFERENCES users(id),
    
    verification_code TEXT,
    is_used BOOLEAN DEFAULT  FALSE,
    expires_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE EXTENSION vector;

CREATE TABLE vehicle_embeddings(
     vehicle_id BIGINT PRIMARY KEY,
     embedding vector(384)
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);

CREATE INDEX idx_drivers_status ON drivers(driver_status);
CREATE INDEX idx_drivers_availability ON drivers(availability);

CREATE INDEX idx_vehicles_status ON vehicles(status);
CREATE INDEX idx_vehicles_brand ON vehicles(brand_id);
CREATE INDEX idx_vehicles_type ON vehicles(type_id);
CREATE INDEX idx_vehicles_price ON vehicles(daily_rental_price);

CREATE INDEX idx_bookings_customer ON bookings(customer_id);
CREATE INDEX idx_bookings_vehicle ON bookings(vehicle_id);
CREATE INDEX idx_bookings_driver ON bookings(driver_id);
CREATE INDEX idx_bookings_status ON bookings(booking_status);
CREATE INDEX idx_bookings_dates ON bookings(pickup_datetime, return_datetime);

CREATE INDEX idx_payments_booking ON payments(booking_id);
CREATE INDEX idx_payments_status ON payments(payment_status);

CREATE INDEX idx_reviews_vehicle ON reviews(vehicle_id);
CREATE INDEX idx_reviews_driver ON reviews(driver_id);

CREATE INDEX idx_notifications_user ON notifications(user_id);
CREATE INDEX idx_notifications_read ON notifications(is_read);