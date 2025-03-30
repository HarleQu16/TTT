using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button[] gridButtons;
    public TMP_Text statusText;
    private string currentPlayer = "X";
    private string[] board = new string[9];
    private bool isSinglePlayer;
    private bool aiMoving;
    public Setting settings;

    private void Start()
    {
        ResetBoard();
        isSinglePlayer = settings.isSinglePlayer;
    }
    
    public void OnCellClicked(int index)
    {
        if (board[index] != "") return;
        if (aiMoving) return;
        if (currentPlayer == "X")
        {
            MakeMove(index, "X");
        }
        
        if (currentPlayer == "O")
        {
            MakeMove(index, "O");
        }
        
        if (!CheckGameOver())
        {
            if(isSinglePlayer)
            {
                StartCoroutine(AIMove());
            }
        }
    }
    
    void MakeMove(int index, string player)
    {
        board[index] = player;
        gridButtons[index].GetComponentInChildren<TMP_Text>().text = player;
        //gridButtons[index].interactable = false;
    }
    
    IEnumerator AIMove()
    {
        aiMoving = true;
        yield return new WaitForSeconds(0.5f);

        int bestMove = GetBestMove();
        if (bestMove != -1)
        {
            MakeMove(bestMove, "O");
            CheckGameOver();
        }
        aiMoving = false;
    }
    
    bool CheckGameOver()
    {
        if (CheckWin("X"))
        {
            statusText.text = "Player X Wins!";
            DisableBoard();
            return true;
        }
        if (CheckWin("O"))
        {
            statusText.text = "Player O Wins!";
            DisableBoard();
            return true;
        }
        if (CheckDraw())
        {
            statusText.text = "Draw!";
            return true;
        }
        SwitchPlayer();
        return false;
    }

    void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == "X") ? "O" : "X";
        statusText.text = $"Player {currentPlayer}'s Turn";
    }
    
    private bool CheckWin(string player)
    {
        int[,] winPatterns = { {0,1,2}, {3,4,5}, {6,7,8}, 
            {0,3,6}, {1,4,7}, {2,5,8}, 
            {0,4,8}, {2,4,6} };
        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            if (board[winPatterns[i, 0]] == player &&
                board[winPatterns[i, 1]] == player &&
                board[winPatterns[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }
    
    bool CheckDraw()
    {
        foreach (string cell in board)
        {
            if (cell == "") return false;
        }
        return true;
    }
    
    void DisableBoard()
    {
        foreach (Button b in gridButtons)
        {
            b.interactable = false;
        }
    }

    public void ResetBoard()
    {
        currentPlayer = "X";
        statusText.text = "Player X's Turn";
        for (int i = 0; i < gridButtons.Length; i++)
        {
            board[i] = "";
            gridButtons[i].GetComponentInChildren<TMP_Text>().text = "";
            gridButtons[i].interactable = true;
        }
    }
    
    int GetBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == "")
            {
                board[i] = "O";
                int score = Minimax(board, 0, false);
                board[i] = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }
        return bestMove;
    }

    int Minimax(string[] newBoard, int depth, bool aiTurn)
    {
        if (CheckWin("O")) return 10 - depth;
        if (CheckWin("X")) return depth - 10;
        if (CheckDraw()) return 0;

        if (aiTurn)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < newBoard.Length; i++)
            {
                if (newBoard[i] == "")
                {
                    newBoard[i] = "O";
                    int score = Minimax(newBoard, depth + 1, false);
                    newBoard[i] = "";
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < newBoard.Length; i++)
            {
                if (newBoard[i] == "")
                {
                    newBoard[i] = "X";
                    int score = Minimax(newBoard, depth + 1, true);
                    newBoard[i] = "";
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }
}
