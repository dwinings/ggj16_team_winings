using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

  public int wallDamage = 1;

  public int pointsPerFood = 10;
  public int pointsPerSoda = 20;
  public float restartLevelDelay = 1f;
  public Text foodText;



  private Animator animator;
  public int food;

	// Use this for initialization
	protected override void Start () {
    animator = GetComponent<Animator>();
    food = GameManager.instance.playerHitPoints;
    UpdateFoodText();
    base.Start();
	}

  private void OnDisable() {
    GameManager.instance.playerHitPoints = food;
  }
	
	// Update is called once per frame
	void Update () {
    if(!GameManager.instance.playersTurn) {
      return;
    }

    int horizontal = 0;
    int vertical = 0;

    horizontal = (int)Input.GetAxisRaw("Horizontal");
    vertical   = (int)Input.GetAxisRaw("Vertical");

    if(horizontal != 0) {
      vertical = 0;
    }

    if (horizontal != 0 || vertical != 0) {
      AttemptMove<Wall>(horizontal, vertical);
    }
	}

  protected override void AttemptMove <T> (int xDir, int yDir) {
    food--;
    foodText.text = string.Format("Food: {0}", food);

    base.AttemptMove<T>(xDir, yDir);
    RaycastHit2D hit;
    CheckIfGameOver();
    GameManager.instance.playersTurn = false;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Exit") {
      Invoke("Restart", restartLevelDelay);
      enabled = false;
    } else if (other.tag == "Food") {
      food += pointsPerFood;
      UpdateFoodText();
      other.gameObject.SetActive(false);
    } else if (other.tag == "Soda") {
      food += pointsPerSoda;
      UpdateFoodText();
      other.gameObject.SetActive(false);
    }
  }

  private void UpdateFoodText() {
    foodText.text = "Food: " + food;
  }

  protected override void OnCantMove<T> (T component) {
    Wall hitWall = component as Wall;
    hitWall.DamageWall(wallDamage);
    animator.SetTrigger("playerChop");
  }

  private void Restart() {
    // hit the exit object
    GameManager.instance.playerHitPoints = food;
    Application.LoadLevel(Application.loadedLevel);
  }

  public void LoseFood(int loss) {
    animator.SetTrigger("playerHit");
    food -= loss;
    CheckIfGameOver();
  }

  private void CheckIfGameOver() {
    if (food <= 0) {
      GameManager.instance.GameOver();
    }
  }
}
