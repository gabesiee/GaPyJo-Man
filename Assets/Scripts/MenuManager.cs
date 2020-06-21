using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.UIElements;

public class MenuManager : Manager<MenuManager>
{

	[Header("MenuManager")]

	#region Panels
	[Header("Panels")]
	[SerializeField] GameObject m_PanelMainMenu;
	[SerializeField] GameObject m_PanelInGameMenu;
	[SerializeField] GameObject m_PanelGameOver;
	[SerializeField] GameObject m_PanelVictory;
	[SerializeField] GameObject m_PanelCredits;

	List<GameObject> m_AllPanels;
	#endregion

	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();
	}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();
	}
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		yield break;
	}
	#endregion

	#region Monobehaviour lifecycle
	protected override void Awake()
	{
		base.Awake();
		RegisterPanels();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			EscapeButtonHasBeenClicked();
		}
	}
	#endregion

	#region Panel Methods
	void RegisterPanels()
	{
		m_AllPanels = new List<GameObject>();
		m_AllPanels.Add(m_PanelMainMenu);
		m_AllPanels.Add(m_PanelInGameMenu);
		m_AllPanels.Add(m_PanelGameOver);
		m_AllPanels.Add(m_PanelVictory);
	}

	void OpenPanel(GameObject panel)
	{
		foreach (var item in m_AllPanels)
			if (item) item.SetActive(item == panel);
	}
	#endregion

	#region UI OnClick Events
	public void EscapeButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new EscapeButtonClickedEvent());
	}

	public void PlayButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new PlayButtonClickedEvent());
	}

	public void ResumeButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new ResumeButtonClickedEvent());
	}

	public void MainMenuButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
	}

	public void QuitButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new QuitButtonClickedEvent());
	}

	public void CreditsButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new CreditsButtonClickedEvent());
	}
    public void DifficultySliderChanged(float difficulty)
    {
        DifficultyValueEvent diff = new DifficultyValueEvent();
        diff.difficulty = (GameDifficulty)difficulty - 1;
        EventManager.Instance.Raise(diff);
    }

    public void EditDifficultyText(DifficultyValueEvent e) {

    }

    #endregion

    #region Callbacks to GameManager events
    protected override void GameMenu(GameMenuEvent e)
	{
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_PanelMainMenu.transform.GetChild(2).gameObject);

        OpenPanel(m_PanelMainMenu);
	}

	protected override void GamePlay(GamePlayEvent e)
	{
		OpenPanel(null);
	}

	protected override void GamePause(GamePauseEvent e)
	{
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_PanelInGameMenu.transform.GetChild(0).gameObject);
        OpenPanel(m_PanelInGameMenu);
	}

	protected override void GameResume(GameResumeEvent e)
	{
		OpenPanel(null);
	}

	protected override void GameOver(GameOverEvent e)
	{
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_PanelGameOver.transform.GetChild(1).gameObject);
        OpenPanel(m_PanelGameOver);
	}

    protected override void GameCredit(GameCreditEvent e)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_PanelCredits.transform.GetChild(1).gameObject);
        OpenPanel(m_PanelCredits);
    }


    #endregion
}
