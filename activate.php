<?php


 if (isset($_POST["serial"])){

	header('Content-Type: application/json; charset=utf-8');
	$my_data = new stdClass();

	// -- query
	$con=mysql_connect('localhost','root','');
	if($con){
		mysql_select_db("fotomoko2",$con)or die(mysql_error());
	}
	
	$s = $_POST["serial"];
	$n = $_POST["nam"];
	$u = $_POST["uniquekey"];
	$m = $_POST["mac"];
	
	
	$query="SELECT * FROM activate where serial='$s' and clientname='$n'";
	$result=mysql_query($query)or die(mysql_error());
	$row=mysql_fetch_array($result);
	
	
	if ($row) {
		// buttontoactivate = true -> pag-kinlick yung Activate button, dapat empty uniqueid and mac
		if ($_POST["buttontoactivate"] == "true"){ 
			if ($row['uniqueid']  == '' and $row['mac'] == ''){
				
				$conn=mysqli_connect('localhost','root','','fotomoko2');
				if(!$conn){die(mysql_error());}
				$query2 = "UPDATE activate SET uniqueid='$u', mac='$m' WHERE (serial='$s' and clientname='$n')";
				if (mysqli_query($conn, $query2)) {
					$my_data->objects = ["111","Activated"];
				} else {
					$my_data->objects = ["fail","Error: 2 - Wrong Activation"];
				}
			}
			else{
				$my_data->objects = ["fail","Error: 1 - Already Activated"];
			}
		}
		// buttontoactivate = false -> nangyayari lang to pag auto-start ng fotomoko, tapos ichcheck nya yung license
		else if ($_POST["buttontoactivate"] == "false"){
			if ($row['uniqueid'] == $u and $row['mac'] == $m){
				$my_data->objects = ["222","Activated"];
			} else{
				$my_data->objects = ["fail","Error: 3"];
			}
		}
	}else{
		$my_data->objects = ["fail","Error: 0"];
	}
	
	echo json_encode($my_data);
	//mysqli_close($conn);
 }

?>