using UnityEngine;
using System.Collections;

public class TradeProgressGraphics : MonoBehaviour {

	public Sprite[] progressSprites;
	public bool isTrading;

	public float startTime;
    private float duration;

	// Update is called once per frame
	void Update () {
		SpriteRenderer sr = GetComponent<SpriteRenderer>();

		if (isTrading)
		{
            float percentage = (Time.time - startTime) / this.duration;
//            Debug.Log("Sprite Percentage : " + percentage);

            //int sprites = progressSprites.Length;

			int spriteIndex = -1;

			float total = progressSprites.Length;
			
//			Debug.Log("Percentage: " + percentage);

			for (int i = 0; i < progressSprites.Length; i++)
			{
				float index = (float)i;

				float spritesPercentage = index / total;
				float spritesPercentagePlus = index + 1 / total;

				if (spritesPercentage <= percentage && spritesPercentagePlus > percentage)
				{
					spriteIndex = i;
				}
//				else if (i == 0 && 
			}

			if (spriteIndex == -1)
			{
				spriteIndex = progressSprites.Length - 1;
			}

//			Debug.Log("SpriteIndex: " + spriteIndex);

			//If we found a Sprite to display, show it here
			if (spriteIndex != -1)
			{
				sr.sprite = progressSprites[spriteIndex];
			}

			if (sr.enabled == false)
			{
				sr.enabled = true;
			}


		}
		else
		{
            //sr.enabled = false;
            //sr.sprite = null;
		}

	}

	public void StartTradingGraphics(float duration)
	{
		isTrading = true;
		startTime = Time.time;
        this.duration = duration;
        GetComponent<SpriteRenderer>().enabled = true;

		Debug.Log("Start Graphics on " + gameObject.GetInstanceID());

	}

	public void StopTradingGraphics()
	{
		isTrading = false;
		GetComponent<SpriteRenderer>().enabled = false;
		
	}
}
