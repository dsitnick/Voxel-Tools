using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelModifier {

    public abstract ParticleSystem.Particle[] Modify (ParticleSystem.Particle[] data);

}

public class Explode : VoxelModifier {

    Vector3 position;
    float velocity;
    bool uniform;

    public Explode(Vector3 position, float velocity, bool uniform){
        this.position = position;
        this.velocity = velocity;
        this.uniform = uniform;
    }

	public override ParticleSystem.Particle[] Modify (ParticleSystem.Particle[] data) {
        for (int i = 0; i < data.Length; i++){
            Vector3 v = data[i].position - position;
            if (uniform){
                v.Normalize ();
                v *= velocity;
            }else{
                float m = v.magnitude;
                v.Normalize ();
                v *= velocity / (m * m);
            }
            data[i].velocity = v;
            data[i].randomSeed = 1;
        }
        return data;
	}
}

public class Slash : VoxelModifier {

    Vector3 position, normal;
    float velocity, displacememnt;

    public Slash(Vector3 position, Vector3 normal, float velocity, float displacement){
        this.position = position;
        this.normal = normal;
        this.velocity = velocity;
        this.displacememnt = displacement;
    }

	public override ParticleSystem.Particle[] Modify (ParticleSystem.Particle[] data) {
        for (int i = 0; i < data.Length; i++){
            float sign = (Vector3.Dot (data[i].position - position, normal) >= 0) ? 1 : -1;

            data[i].position += normal * displacememnt * sign;
            data[i].velocity = normal * velocity * sign;
        }
        return data;
	}
}
