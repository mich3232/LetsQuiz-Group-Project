<?php
    // server variables    
    $_host = "localhost";
	$_username =  "id5267535_admin";
	$_password = "letsquiz";
	$_db = "id5267535_letsquiz";
	
	$data = $_GET["data"];
	
	// establish server connection
	$connection = new mysqli($_host, $_username, $_password, $_db);
	
	// check connection is established
	if ($connection->connect_error)
	{
		die( "Error : ".$connection->connect_error);
	}
	
	// insert new data from tech_comp
	$insert = "INSERT INTO tech_comp (tech_comp_data) VALUES ('$data');";
	
	if($connection->query($insert) === true)
	{
		echo "Record added successfully.";
	}
	else
	{
		echo "Error : ".$connection->error;
	}
	
	$connection->close();
?>