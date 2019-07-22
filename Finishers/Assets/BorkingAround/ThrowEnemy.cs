using Finisher.Characters.Player.Finishers;
using System.Collections;
using UnityEngine;

public class ThrowEnemy : MonoBehaviour
{
    public float movementSpeed = 10f;
    public FlameAOE flameAOE = null;

    public IEnumerator ThrowEnemyCoroutine()
    {
        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        while (true)
        {
            transform.position += transform.forward * -2 * movementSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Obstructive" || collision.gameObject.tag == "Enemy"))
        {
            if (flameAOE != null)
            {
                Instantiate(flameAOE, transform.position, transform.rotation);
            }
            StopAllCoroutines();
            print("Explode");
            Destroy(gameObject);
        }
    }
}