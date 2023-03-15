using UnityEngine;

public class LootBox : MonoBehaviour
{
    public GameObject ammoPrefab;
    public float dropChance = 0.5f;
    public int cost = 30;

    private coins Coins;

    private void Start()
    {
        Coins = FindObjectOfType<coins>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    AttemptLoot();
                }
            }
        }
    }

    private void AttemptLoot()
    {
        if (Coins.CoinAmmount >= cost)
        {
            Coins.RemoveCoins(cost);
            if (Random.value <= dropChance)
            {
                DropAmmo();
            }
            Destroy(gameObject);
        }
    }

    private void DropAmmo()
    {
        Instantiate(ammoPrefab, transform.position + Vector3.up, Quaternion.identity);
    }
}
