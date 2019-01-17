<?php
    // server    
    $_host = "localhost";
	$_username =  "id5267535_admin";
	$_password = "letsquiz";
	$_db = "id5267535_letsquiz";
	
	// establish server connection
	$connection = new mysqli($_host, $_username, $_password, $_db);
	// Check connection
	if ($connection->connect_error)
	{
		die("Error: " . $connection->connect_error);
	} 

	// delete data from tech_comp
	$delete = "DELETE FROM tech_comp;";
	$result = $connection->query($delete);

	if($connection->query($delete) === true)
	{
		echo "Data deleted successfully.";
	}
	else
	{
		echo "Error : ".$connection->error;
	}
	$connection->close();
?>