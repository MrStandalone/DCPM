using UnityEngine;

class WireframeScript : MonoBehaviour
{
	void OnPreRender()
	{
		GL.wireframe = true;
	}
	void OnPostRender()
	{
		GL.wireframe = false;
	}
}

