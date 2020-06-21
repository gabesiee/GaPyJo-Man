﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SDD.Events;
using UnityEngine.SceneManagement;

public enum GameState { gameMenu, gamePlay, gameNextLevel, gamePause, gameOver, gameVictory }
public enum GameDifficulty { Easy, Normal, Hard, Harder }

public class GameManager : Manager<GameManager>
{
	#region Game State
	private GameState m_GameState;
	public bool IsPlaying { get { return m_GameState == GameState.gamePlay; } }
	#endregion

	public GameObject HUDPanel;
	public GameObject mainMenuPanel;
	public GameObject pausePanel;
	public GameObject VictoryPanel;
	public GameObject gameOverPanel;
	public GameObject creditsPanel;
	public Text scoreText;





    private GameDifficulty difficulty = GameDifficulty.Easy;


	#region Score
	private float m_Score;
    public float Score
    {
		get { return m_Score; }
		set
		{
			m_Score = value;
		}
	}
   
	void IncrementScore(float increment)
	{
		SetScore(m_Score + increment);
	}

	void SetScore(float score, bool raiseEvent = true)
	{
		Score = score;
        if (SfxManager.Instance) SfxManager.Instance.PlaySfx2D(Constants.SCORE_SFX);
	}
	#endregion

	public static int scoreToWin;

	#region Time
	void SetTimeScale(float newTimeScale)
	{
		Time.timeScale = newTimeScale;
	}
	#endregion


	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();

		EventManager.Instance.AddListener<PlayerHasBeenHitEvent>(PlayerHasBeenHit);

		//MainMenuManager
		EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
		EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
		EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
		EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
		EventManager.Instance.AddListener<QuitButtonClickedEvent>(QuitButtonClicked);
		EventManager.Instance.AddListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        EventManager.Instance.AddListener<DifficultyValueEvent>(SetDifficulty);

        //Score Item
        EventManager.Instance.AddListener<ScoreItemEvent>(ScoreHasBeenGained);


		EventManager.Instance.AddListener<GameMusicEvent>(toggleMusic);


	}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();

		EventManager.Instance.RemoveListener<PlayerHasBeenHitEvent>(PlayerHasBeenHit);

		//MainMenuManager
		EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
		EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);
		EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
		EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
		EventManager.Instance.RemoveListener<QuitButtonClickedEvent>(QuitButtonClicked);
		EventManager.Instance.RemoveListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        EventManager.Instance.RemoveListener<DifficultyValueEvent>(SetDifficulty);

        //Score Item
        EventManager.Instance.RemoveListener<ScoreItemEvent>(ScoreHasBeenGained);


		EventManager.Instance.RemoveListener<GameMusicEvent>(toggleMusic);
	}
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		Menu();
		InitNewGame(); // essentiellement pour que les statistiques du jeu soient mise à jour en HUD
		yield break;
	}
	#endregion

	#region Game flow & Gameplay
	//Game initialization
	void InitNewGame(bool raiseStatsEvent = true)
	{
		SetScore(0);
	}

    public void SetDifficulty(DifficultyValueEvent e)
    {
        difficulty = e.difficulty;
    }
    #endregion

    #region Callbacks to events issued by Score items
    private void ScoreHasBeenGained(ScoreItemEvent e)
	{
		IncrementScore(1);
		scoreText.text = m_Score + "/" + scoreToWin;

		if (m_Score == scoreToWin)
        {
			Victory();
        }
			
	}
	#endregion

	#region Callbacks to Events issued by MenuManager
	private void PlayerHasBeenHit(PlayerHasBeenHitEvent e)
	{
		Over();
	}

	private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
	{
		scoreToWin = 0;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Menu();
	}

	private void PlayButtonClicked(PlayButtonClickedEvent e)
	{
        if (SfxManager.Instance) SfxManager.Instance.PlaySfx2D(Constants.BUTTON_SFX);
        Play();
	}

	private void ResumeButtonClicked(ResumeButtonClickedEvent e)
	{
		Resume();
	}

	private void EscapeButtonClicked(EscapeButtonClickedEvent e)
	{
		if (IsPlaying) Pause();
	}

	private void QuitButtonClicked(QuitButtonClickedEvent e)
	{
		Application.Quit();
	}

	private void CreditsButtonClicked(CreditsButtonClickedEvent e)
	{
		credit();
	}
	#endregion

	#region GameState methods
	private void Menu()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameMenu;

		if(MusicLoopsManager.Instance)MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
		EventManager.Instance.Raise(new GameMenuEvent());
	}

	private void Play()
	{
		InitNewGame();
        MapManager.Instance.setEnnemies((int)difficulty + 1);
        SetTimeScale(1);
		
		mainMenuPanel.SetActive(false);
		HUDPanel.SetActive(true);
        


        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_MUSIC);
		EventManager.Instance.Raise(new GamePlayEvent());
        m_GameState = GameState.gamePlay;
    }

	private void Pause()
	{
		if (!IsPlaying) return;

		SetTimeScale(0);
		m_GameState = GameState.gamePause;
        if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.StopAllRightAway();
        Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;


		EventManager.Instance.Raise(new GamePauseEvent());
	}

	private void Resume()
	{
		if (IsPlaying) return;

		SetTimeScale(1);
		m_GameState = GameState.gamePlay;
        if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayCurrentMusic();
       Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		EventManager.Instance.Raise(new GameResumeEvent());
	}

	private void Over()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameOver;

		gameOverPanel.SetActive(true);
		HUDPanel.SetActive(false);

		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;

		EventManager.Instance.Raise(new GameOverEvent());
        if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_GAMEOVER);
        if (SfxManager.Instance) SfxManager.Instance.PlaySfx2D(Constants.GAMEOVER_SFX);
	}

	private void Victory()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameVictory;

		VictoryPanel.SetActive(true);
		HUDPanel.SetActive(false);

		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;

		EventManager.Instance.Raise(new GameVictoryEvent());
        if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_VICTORY);
        if (SfxManager.Instance) SfxManager.Instance.PlaySfx2D(Constants.VICTORY_SFX);
	}

    private int musicState = 0;
    private void toggleMusic(GameMusicEvent e)
    {
        if (e.type == 0 && musicState >= 1)
        {
            musicState--;
        }
        else if (e.type == 1)
        {
            if (SfxManager.Instance) SfxManager.Instance.PlaySfx2D(Constants.SCREAM_SFX);
            musicState++;
        }

        if (musicState == 1)
        {
            if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_HUNT);
        }
        if (musicState == 0)
        {
            if (MusicLoopsManager.Instance) MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_MUSIC);
        }

    }

	private void credit()
    {
		SetTimeScale(0);
		m_GameState = GameState.gameMenu;

		creditsPanel.SetActive(true);
		mainMenuPanel.SetActive(false);

		
		EventManager.Instance.Raise(new GameCreditEvent());
	}

    #endregion
}

