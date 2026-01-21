<?php
$host = "localhost";
$user = "root";
$password = "";
$db = "registration_db";

// Connect to DB
$conn = new mysqli($host, $user, $password, $db);
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Get form data
$full_name = $_POST['full_name'];
$email = $_POST['email'];
$phone = $_POST['phone'];
$birth_date = $_POST['birth_date'];
$gender = $_POST['gender'];
$address1 = $_POST['address1'];
$address2 = $_POST['address2'];
$country = $_POST['country'];
$city = $_POST['city'];
$region = $_POST['region'];
$postal_code = $_POST['postal_code'];

// Insert into DB
$sql = "INSERT INTO users (full_name, email, phone, birth_date, gender, address1, address2, country, city, region, postal_code)
VALUES ('$full_name', '$email', '$phone', '$birth_date', '$gender', '$address1', '$address2', '$country', '$city', '$region', '$postal_code')";

if ($conn->query($sql) === TRUE) {
  echo "🎉 Registration successful!";
} else {
  echo "❌ Error: " . $sql . "<br>" . $conn->error;
}

$conn->close();
?>
