using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] GameObject meteorParticle;
    [SerializeField] GameObject explosionParticle;

    [SerializeField] List<ParticleSystem> velocityOffsetSystems;

    [SerializeField] float explosionDuration = 3;
    [SerializeField] float initForce = 100;
    [SerializeField] Vector3 initTorque = Vector3.zero;
    [SerializeField] float acceleration = 0;
    [SerializeField] Vector3 torque = Vector3.zero;

    Rigidbody rbody;
    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        explosionParticle.SetActive(false);
    }
    private void Start()
    {
        LaunchMeteor();
    }
    private void FixedUpdate()
    {
        MoveMeteor();
        foreach (ParticleSystem ps in velocityOffsetSystems)
        {
            var shape = ps.shape;
            Vector3 v = new Vector3(0, 0, rbody.velocity.magnitude * 0.05f);
            shape.position = v;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint p = collision.GetContact(0);
        
        StartCoroutine(Explode(p));
    }

    void LaunchMeteor()
    {
        Vector3 dir = transform.forward;
        rbody.AddForce(initForce * dir, ForceMode.Impulse);
        rbody.AddTorque(initTorque, ForceMode.Impulse);
    }
    void MoveMeteor()
    {
        Vector3 dir = transform.forward;
        rbody.AddForce(acceleration*dir, ForceMode.Acceleration);
        rbody.AddForce(torque, ForceMode.Acceleration);
    }
    IEnumerator Explode(ContactPoint p)
    {
        Quaternion r = Quaternion.Euler(p.normal);
        //meteorParticle.SetActive(false);
        ParticleSystem[] meteor_ps = meteorParticle.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem s in meteor_ps)
        {
            s.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        meteor_ps[0].Clear(false);
        rbody.isKinematic = true;

        explosionParticle.transform.SetPositionAndRotation(p.point+ p.normal * .3f, r);
        explosionParticle.SetActive(true);



        yield return new WaitForSeconds(explosionDuration-0.01f);
        ParticleSystem[] explosion_ps = explosionParticle.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem s in explosion_ps)
        {
            s.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
