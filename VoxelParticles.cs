using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelParticles : MonoBehaviour {

    //Every mesh maxes at 65535 vertices, and with 24 verts per cube, this represents the max voxels per system
    public const int MAXSIZE = 65535 / 24;
    public const float GRAVITY = 9.8f;

    //ParticleSystem prefab with ideal settings
    public GameObject Prefab;
    public ParticleSystem.Particle[] particles { get; private set; }

    private ParticleSystem[] systems = new ParticleSystem[0];

    //Used to access and modify the particle state
    //Assumes the number of particles remain the same
    public ParticleSystem.Particle[] Particles {
        get {
            return particles == null ? new ParticleSystem.Particle[0] : particles;
        }
        set {
            SetParticles (value);
        }
    } 

    //Given a VoxelModel, creates the particle systems and writes the particle data to them
    public void Build (VoxelModel model) {
        Particles = model.BuildParticles ();
    }

    private void OnEnable () {
        Particles = Particles;
    }

    private void Update () {
        for (int i = 0; i < particles.Length; i++) {
            particles[i].position += particles[i].velocity * Time.deltaTime;

            if ((particles[i].randomSeed & 1) != 0){
                particles[i].velocity -= Vector3.up * GRAVITY * Time.deltaTime;
            }

        }
        Particles = particles;
    }

    //Sets up the necessary particle systems to render the given data
    //Allows for dynamic deletion and addition of particle systems, according to what is necessary
    private void SetParticles(ParticleSystem.Particle[] data){

        int systemCount = Mathf.CeilToInt (data.Length / ((float)MAXSIZE));

        ParticleSystem[] lastSystems = systems;
        systems = new ParticleSystem[systemCount];

        int offset = 0;
        ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[MAXSIZE];

        for (int i = 0; i < systemCount; i++){
            if (i < lastSystems.Length){
                //Use previous particle system
                systems[i] = lastSystems[i];
            }else{
                //Create new particle system
                //Does not invoke unless there are now more systems
                systems[i] = Instantiate (Prefab, transform).GetComponent<ParticleSystem> ();
            }

            int remaining = Mathf.Min (MAXSIZE, data.Length - offset);

            //Copies particles to buffer
            for (int p = 0; p < remaining; p++) {
                buffer[p] = data[p + offset];
            }

            //Key function to set up the particle data for the system
            systems[i].SetParticles (buffer, remaining);

            offset += MAXSIZE;
        }
        //For any extra particle systems to be destroyed
        //Does not invoke unless there are now fewer systems
        for (int i = systemCount; i < lastSystems.Length; i++){
            Destroy (lastSystems[i].gameObject);
        }

        particles = data;
    }
}
