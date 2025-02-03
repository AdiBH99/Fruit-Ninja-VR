using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EzySlice;

enum RemoveType
{
    FRUIT,
    FRAGMENTS
}

public class Fruit : MonoBehaviour
{
    Rigidbody fruitRigidbody;

    bool was_deleted = false;

    bool sword_plane1_collision = false;  // Will be true if there was a collision with side 1 of the sword
    bool sword_plane2_collision = false;  // Will be true if there was a collision with side 2 of the sword
    bool sword_stab = false;  // Will be true if there was a collision with the tip of the sword

    bool is_slicing_now = false; // Will be true during the slice so the slice function won't be called multiple times in a row

    public bool is_bomb = false;

    bool on_ground = false;

    public int num_of_fragments = 3; // Number of fragments along one axis
    public float explosionForce = 10f; // Force applied to fragments
    public float explosionRadius = 3f; // Radius of the explosion

    public GameObject sword_object;
    public Material material;
    public GameObject sliced_part1;
    public GameObject sliced_part2;

    public SoundController soundController;

    private ScoreController scoreController;

    private LivesController livesController;

    public GameObject explosionEffect; 

    // Start is called before the first frame update
    void Start()
    {
        fruitRigidbody = transform.GetComponent<Rigidbody>();
        soundController = SoundController.instance;
        scoreController = FindObjectOfType<ScoreController>();
        livesController = FindObjectOfType<LivesController>();
    }

    private bool had_sword_collision() {
        return sword_stab || sword_plane1_collision || sword_plane2_collision;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("entered OnCollisionEnter! " + GetInstanceID() + " is slicing now: " + is_slicing_now + " is sword stab: " + sword_stab);
        if (sword_stab || (sword_plane1_collision && sword_plane2_collision) || is_slicing_now) {
            return;
        }   
        if (collision.gameObject.CompareTag("Fragment"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("Ground"))
        {    
            soundController.FruitImpactSound();
            on_ground = true;
            StartCoroutine(RemoveAfterSeconds(3f, RemoveType.FRUIT));
        }
        if (collision.gameObject.CompareTag("SwordTip") && !had_sword_collision())
        {
            sword_stab = true;
            is_slicing_now = true;
            Debug.Log("Collision with tip");
        }
        if (collision.gameObject.CompareTag("SwordPlane1"))
        {
            sword_plane1_collision = true;
            is_slicing_now = sword_plane1_collision && sword_plane2_collision;
            Debug.Log("Collision with plane1");
            Debug.LogWarning(collision.gameObject);
            Debug.LogWarning(collision.gameObject.transform);
            Debug.LogWarning(collision.gameObject.transform.parent);
            Debug.LogWarning(collision.gameObject.transform.parent.gameObject);
            if (sword_object == null) {
                Debug.Log("Extracting sword object after collision");
                sword_object = collision.gameObject.transform.parent.gameObject;
                Debug.Log(sword_object);
            }
        }
        if (collision.gameObject.CompareTag("SwordPlane2"))
        {
            sword_plane2_collision = true;
            is_slicing_now = sword_plane1_collision && sword_plane2_collision;
            Debug.Log("Collision with plane2");
            if (sword_object == null) {
                Debug.Log("Extracting sword object after collision");
                sword_object = collision.gameObject.transform.parent.gameObject;
                Debug.Log(sword_object);
            }
        }
        checkCollision();
    }

    private void checkCollision()
    {
        if (on_ground)
        {
            return;
        }
        if (is_bomb && (sword_stab || (sword_plane1_collision && sword_plane2_collision))) {
            if (!livesController.isLiveMode) {
                scoreController.AddPoints(-5);
            }
            livesController.ReduceLife();
            scoreController.IncrBombs();
            soundController.BombSound();
            Explode();
        }
        else if (sword_stab){
            Debug.Log("Stab activate for " + GetInstanceID());
            soundController.StabSound();
            scoreController.AddPoints(5);
            scoreController.IncrStabs();
            Shatter();
            Debug.Log("Adding points for" + GetInstanceID());
        }
        else if (sword_plane1_collision && sword_plane2_collision) {
            // slice and add slice sound
            Debug.Log("Slice activate for " + GetInstanceID());
            soundController.FruitSliceSound();
            scoreController.AddPoints(3);
            scoreController.IncrSlices();
            SliceFruit();
        }
    }

    private void Explode()
    {
        GameObject explosion_object = Instantiate(explosionEffect, transform.position, transform.rotation);
        StartCoroutine(RemoveAfterSeconds(0f, RemoveType.FRUIT));
        Destroy(explosion_object, 2f);
    }

    private void SliceFruit()
    {
        // Calculate the slicing plane
        if (sword_object == null) {
                Debug.LogWarning("Null Error inside slice"); 
        }

        Sword sword_plane_holder = sword_object.GetComponent<Sword>();
        UnityEngine.Plane slicePlane = new UnityEngine.Plane(this.gameObject.transform.up, this.gameObject.transform.position);
        Vector3 swordUpPosition = sword_object.GetComponent<Transform>().up;
        Vector3 forceDirection = slicePlane.normal;

        Vector3 fruitPosition = transform.position;
        Quaternion fruitRotation = transform.rotation;

        GameObject upper_slice = Instantiate(sliced_part1, fruitPosition, fruitRotation);
        GameObject lower_slice = Instantiate(sliced_part2, fruitPosition, fruitRotation);

        StartCoroutine(RemoveAfterSeconds(0f, RemoveType.FRUIT));
        upper_slice.tag = "Fragment";
        lower_slice.tag = "Fragment";
        AddForceToHull(upper_slice, forceDirection, 0.3f);
        AddForceToHull(lower_slice, -forceDirection, 0.3f);

        Destroy(upper_slice, 10f);
        Destroy(lower_slice, 10f);

    }

    private void SliceFruitOld()
    {
        // Calculate the slicing plane
        if (sword_object == null) {
                Debug.LogWarning("Null Error inside slice"); 
        }
        
        Sword sword_plane_holder = sword_object.GetComponent<Sword>();
        UnityEngine.Plane slicePlane = new UnityEngine.Plane(this.gameObject.transform.up, this.gameObject.transform.position);
        Vector3 swordUpPosition = sword_object.GetComponent<Transform>().up;
        Debug.Log(this.gameObject.transform.position);
        Debug.Log(swordUpPosition);
        Debug.Log(this.gameObject.transform.up);
        
        SlicedHull hull = null;
        Vector3 fruitPosition = new Vector3();
        // for (float i = -1f; i < 1f; i+=0.01f) {
        //     Vector3 offset = new Vector3(i, i, i);
        //     fruitPosition = this.gameObject.transform.position + offset;
        //     hull = this.gameObject.Slice(fruitPosition, swordUpPosition, material);
        //     if (hull != null) {
        //         break;
        //     }
        // }
        // if (hull == null) {
        //     Debug.LogWarning("Hull is still null");
        // }
        fruitPosition = this.gameObject.transform.position;
        hull = this.gameObject.Slice(fruitPosition, swordUpPosition, material);
        Debug.Log(hull);
        if (hull != null) {
            // Generate sliced fruit pieces
            GameObject upper_hull = hull.CreateUpperHull(this.gameObject, material);
            GameObject lower_hull = hull.CreateLowerHull(this.gameObject, material);
            Mesh upperMesh = upper_hull.GetComponent<MeshFilter>().mesh;
            Mesh lowerMesh = lower_hull.GetComponent<MeshFilter>().mesh;
            // AddClosingPlanes(upperMesh, lowerMesh, slicePlane);
            StartCoroutine(RemoveAfterSeconds(0f, RemoveType.FRUIT));
            upper_hull.tag = "Fragment";
            lower_hull.tag = "Fragment";
            Debug.Log("Created 2 pieces!");

            // Vector3 forceDirection = (upper_hull.transform.position - lower_hull.transform.position).normalized;
            // Vector3 forceDirection = swordUpPosition.normalized;
            Vector3 forceDirection = slicePlane.normal;
            // Quaternion rotation = Quaternion.Euler(0, 0, 90);
            // forceDirection = rotation * forceDirection;
            Debug.Log(forceDirection);
            upper_hull.transform.position = this.gameObject.transform.position;
            lower_hull.transform.position = this.gameObject.transform.position;
            AddForceToHull(upper_hull, forceDirection, 0.3f);
            AddForceToHull(lower_hull, -forceDirection, 0.3f);

            Destroy(upper_hull, 10f);
            Destroy(lower_hull, 10f);
        }

        // GenerateFruitPieces(slicePlane);

    }

        void AddClosingPlanes(Mesh topMesh, Mesh bottomMesh, UnityEngine.Plane slicePlane)
    {
        // Add closing plane logic here
        // This can involve creating new vertices at the slice edges and forming a new mesh to close the gaps
        // You can use the sliceMaterial to create the plane surfaces

        // This is a placeholder implementation and needs to be expanded
        GameObject topPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        topPlane.transform.position = slicePlane.ClosestPointOnPlane(topMesh.bounds.center);
        topPlane.transform.rotation = Quaternion.LookRotation(slicePlane.normal);
        topPlane.transform.localScale = new Vector3(topMesh.bounds.size.x, 1, topMesh.bounds.size.z);
        topPlane.GetComponent<Renderer>().material = material;

        GameObject bottomPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        bottomPlane.transform.position = slicePlane.ClosestPointOnPlane(bottomMesh.bounds.center);
        bottomPlane.transform.rotation = Quaternion.LookRotation(slicePlane.normal);
        bottomPlane.transform.localScale = new Vector3(bottomMesh.bounds.size.x, 1, bottomMesh.bounds.size.z);
        bottomPlane.GetComponent<Renderer>().material = material;
    }

    void AddForceToHull(GameObject hull, Vector3 forceDirection, float forceMagnitude) {
        
        hull.AddComponent<BoxCollider>();        
        hull.AddComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        hull.GetComponent<Rigidbody>().AddForce(forceMagnitude * forceDirection, ForceMode.Impulse);
    }

    
    void AddForceToHullOld(GameObject hull, Vector3 forceDirection, float forceMagnitude) {

        // Rigidbody rigidbody = hull.GetComponent<Rigidbody>();
        // if (rigidbody == null)
        // {
        //     rigidbody = hull.AddComponent<Rigidbody>();
        // }
        
        hull.AddComponent<BoxCollider>();        
        Material[] materials = hull.GetComponent<MeshRenderer>().materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = material;
        }
        hull.GetComponent<MeshRenderer>().materials = materials;
        hull.AddComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        // hull.GetComponent<Rigidbody>().AddExplosionForce(forceMagnitude, forceDirection, 30);
        hull.GetComponent<Rigidbody>().AddForce(forceMagnitude * forceDirection, ForceMode.Impulse);
        // float forceMagnitude = 300f; // Adjust this value to control the force applied
        // rigidbody.AddForce(forceDirection * forceMagnitude * (1f / Time.timeScale));
    }

    // void GenerateFruitPieces(Plane slicePlane)
    // {
    //     // Create two new fruit pieces

    //     GameObject fruitPiece1, fruitPiece2;
    //     MeshSlicer.SliceMesh(gameObject, slicePlane, out fruitPiece1, out fruitPiece2);
    //     fruitPiece1.tag = "Fragment";
    //     fruitPiece2.tag = "Fragment";

    //     // Destroy the original fruit
    //     StartCoroutine(RemoveAfterSeconds(0f, RemoveType.FRUIT));
    //     Destroy(fruitPiece1, 6f);
    //     Destroy(fruitPiece2, 6f);
    // }

    /**
    ***** Shatter to many small cubes
    **/
    private void Shatter()
    {
        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;
        Vector3 originalScale = transform.localScale;
        StartCoroutine(RemoveAfterSeconds(0f, RemoveType.FRUIT));


        float fragmentSize = originalScale.x / num_of_fragments;
        fragmentSize = Mathf.Min(fragmentSize, (0.25f / 3f));
        // int fragments_counter = 0;
        for (int x = 0; x < num_of_fragments; x++)
        {
            for (int y = 0; y < num_of_fragments; y++)
            {
                for (int z = 0; z < num_of_fragments; z++)
                {
                    CreateFragment(x, y, z, fragmentSize, originalPosition, originalRotation);
                }
            }
        }
    }

    private void CreateFragment(int x, int y, int z, float size, Vector3 position, Quaternion rotation)
    {
        GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fragment.tag = "Fragment";
        fragment.transform.localScale = new Vector3(size, size, size);
        fragment.transform.position = position + new Vector3(x * size, y * size, z * size) - new Vector3(size * num_of_fragments / 2f, size * num_of_fragments / 2f, size * num_of_fragments / 2f);
        fragment.transform.rotation = rotation;

        fragment.AddComponent<Rigidbody>().AddExplosionForce(explosionForce, position, explosionRadius);
        // fragment.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        fragment.GetComponent<Renderer>().material = material;
        Destroy(fragment, 4f);
    }

    private IEnumerator RemoveAfterSeconds(float seconds, RemoveType remove_type)
    {
        yield return new WaitForSeconds(seconds);
        if (remove_type == RemoveType.FRAGMENTS) {
            // foreach (GameObject fragment in fragments)
            // {
            //     Destroy(fragment);
            // }
        }
        else { // remove_type == RemoveType.FRUIT
            ResetVelocity();
        }
    }


    public void ResetVelocity()
    {
        was_deleted = true;
        gameObject.SetActive(false);
        fruitRigidbody.velocity = Vector3.zero;
        fruitRigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        fruitRigidbody.interpolation = RigidbodyInterpolation.None;
    }

    public bool IsDeleted()
    {
        return was_deleted;
    }

    public void ResetDeleteMode()
    {
        was_deleted = false;
        sword_stab = false;
        sword_plane1_collision = false;
        sword_plane2_collision = false;
        is_slicing_now = false;
        sword_object = null;
        on_ground = false;
    }

}
