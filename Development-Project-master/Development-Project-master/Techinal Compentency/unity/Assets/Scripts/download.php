<?php
    // server    
    $_host = "localhost";
	$_username =  "id5267535_admin";
	$_password = "letsquiz";
	$_db = "id5267535_letsquiz";
	
	// establish server connection
	$connection = new mysqli($_host, $_username, $_password, $_db);
	
	// check connection is established
	if ($connection->connect_error)
	{
		die( "Error : ".$connection->connect_error);
	}	
	
	// select all from tech_comp
	$select = "SELECT * FROM tech_comp";
	

	// perform the create
	$result = $connection->query($select);
	
	// check there are rows
	if ($result->num_rows > 0)
	{
		// iterate through the rows
		while($row = $result->fetch_assoc())
		{
		    // return result
			echo $row['tech_comp_data']."\n";
		}
	}
	else 
	{
	    echo "No data found in the database.";
	}
	
	mysqli_close($connection);
?>