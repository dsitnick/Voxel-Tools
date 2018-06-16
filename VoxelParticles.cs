using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelParticles : MonoBehaviour {

    //Every mesh maxes at 65535 vertices, and with 24 verts per cube, this represents the max voxels per system
    public const int MAXSIZE = 65535 / 24;

    //ParticleSystem prefab with ideal settings
    public GameObject Prefab;
    public ParticleSystem.Particle[] particles { get; private set; }

    private ParticleSystem[] systems = new ParticleSystem[0];

    //Given a VoxelModel, creates the particle systems and writes the particle data to them
    public void Build(VoxelModel model){
        //Clears previous particle systems
        for (int i = 0; i < systems.Length; i++){
            Destroy (systems[i].gameObject);
        }

        //Builds the particles to a buffer
        particles = model.BuildParticles ();

        //Creates just enough particle systems
        int count = Mathf.CeilToInt (particles.Length / ((float)MAXSIZE));
        systems = new ParticleSystem[count];

        //Buffer is used to store particle data for each system
        ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[MAXSIZE];

        //Index offset for each system
        int offset = 0;

        for (int i = 0; i < count; i++){
            //Creates the particle system from the prefab
            systems[i] = Instantiate (Prefab, transform).GetComponent<ParticleSystem> ();

            //Represents the number of particles for this system
            int remaining = Mathf.Min (MAXSIZE, particles.Length - offset);

            //Copies particles to buffer
            for (int p = 0; p < remaining; p++){
                buffer[p] = particles[p + offset];
            }

            //Key function to set up the particle data for the system
            systems[i].SetParticles (buffer, remaining);

            offset += MAXSIZE;
        }
    }
}
