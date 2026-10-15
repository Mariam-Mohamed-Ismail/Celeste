using UnityEngine;

using System.Collections;



public class SnowCubeFalling : MonoBehaviour

{

    private bool playerInside = false;

    private bool hasFallen = false;

    private Rigidbody rb;



    void Start()

    {

        rb = GetComponent<Rigidbody>();

        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true; // Stay in place initially

    }



    void OnTriggerEnter(Collider other)

    {

        if (other.CompareTag("Player")) playerInside = true;

    }



    void OnTriggerExit(Collider other)

    {

        if (other.CompareTag("Player") && playerInside && !hasFallen)

        {

            StartCoroutine(FallBehindPlayer());

        }

    }



    IEnumerator FallBehindPlayer()

    {

        hasFallen = true;

        yield return new WaitForSeconds(0.1f); // Tiny delay to feel like it's falling "behind"

        rb.isKinematic = false;

        rb.useGravity = true;

    }

}