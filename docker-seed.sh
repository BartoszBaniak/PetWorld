#!/bin/bash
# Script to seed the database with product data

echo "Waiting for MySQL to be ready..."
sleep 5

echo "Seeding product catalog..."
docker exec -i petworld-mysql mysql -uroot -ppetworld123 PetWorldDb < seed-products.sql

if [ $? -eq 0 ]; then
    echo "✓ Products seeded successfully!"
else
    echo "✗ Failed to seed products"
    exit 1
fi
