using System;
using System.Data.SqlClient; 
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string connectionString = "Server=Probook455\\SQLEXPRESS,1433;Database=2048;Integrated Security=True;Encrypt=False;";


    public void TestConnection()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                Debug.Log("Connected to SQL Server successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError("Database connection error: " + ex.Message);
            }
        }
    }
    public int InsertPlayer(string playerName, int score)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string checkQuery = "SELECT id FROM player WHERE name = @name";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@name", playerName);
                    var existingPlayerId = checkCmd.ExecuteScalar();

                    if (existingPlayerId != null)
                    {
                        Debug.Log("Player already exists! Returning existing PlayerID.");
                        return Convert.ToInt32(existingPlayerId);
                    }
                }
                string insertQuery = "INSERT INTO player (name, hiScore) OUTPUT INSERTED.id VALUES (@name, @hiScore)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@name", playerName);
                    cmd.Parameters.AddWithValue("@hiScore", score);

                    var newPlayerId = cmd.ExecuteScalar();

                    Debug.Log("Player inserted successfully!");
                    return Convert.ToInt32(newPlayerId);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error inserting player: " + ex.Message);
                return -1;
            }
        }
    }


    public void InsertGame(int playerId, int score, int bestNum)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                // Step 1: Insert the new game
                string insertQuery = "INSERT INTO games (score, bestNum, playerId) VALUES (@score, @bestNum, @playerId)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@score", score);
                    insertCmd.Parameters.AddWithValue("@bestNum", bestNum);
                    insertCmd.Parameters.AddWithValue("@playerId", playerId);
                    insertCmd.ExecuteNonQuery();
                    Debug.Log("Game inserted successfully!");
                }

                // Step 2: Check if the new score is higher than the player's hiScore
                string updateQuery = @"
                    UPDATE player
                    SET hiScore = @score
                    WHERE id = @playerId AND hiScore < @score";

                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@score", score);
                    updateCmd.Parameters.AddWithValue("@playerId", playerId);
                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    
                    if (rowsAffected > 0)
                    {
                        Debug.Log("Player's hiScore updated!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error inserting game: " + ex.Message);
            }
        }
    }

    public int GetHiScore(int playerId)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string query = "SELECT hiScore FROM player WHERE id = @playerId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@playerId", playerId);
                    
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        Debug.Log("Player not found or hiScore is null.");
                        return 0; // Return 0 if player doesn't exist or has no hiScore
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error retrieving hiScore: " + ex.Message);
                return -1; // Return -1 to indicate an error
            }
        }
    }





}