<?php
$servername = "localhost";
$username = "user";
$password = "hvdifugthserltig";
$dbname = "id9298481_sciencebirds";

$secretKey = "aiingames";
$hash = $_GET['hash'];

$realHash = md5($_GET['LSystemId'] . $_GET['rating'] . $secretKey);
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
	die("Connection failed: " . $conn->connect_error);
}

if ($realHash == $hash) {
	$stmt = $conn->prepare("INSERT INTO LSystems Values(?,?)");
	$stmt->bind_param('ii', $_GET["LSystemId"], $_GET["rating"]);

	if (isset($stmt)) {
		$return = $stmt->execute() or die("Rating update failed");
	}
}
?>