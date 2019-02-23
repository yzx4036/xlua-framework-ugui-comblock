using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DamageNumber:MonoBehaviour 
	{
		public TextMesh textMesh;

		protected RexPool parentSpawnPool;

		public void Show(int damageAmount, Vector2 _position, RexPool _parentSpawnPool)
		{
			textMesh.text = damageAmount.ToString();
			transform.position = _position;
			parentSpawnPool = _parentSpawnPool;

			StartCoroutine("ShowCoroutine");
		}

		protected IEnumerator ShowCoroutine()
		{
			textMesh.gameObject.SetActive(true);

			float speed = 1.75f;
			float destinationY = transform.position.y + 1.5f;
			while(transform.position.y <= destinationY)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime), 0.0f);

				if(transform.position.y > destinationY)
				{
					transform.position = new Vector3(transform.position.x, destinationY, 0.0f);
					break;
				}

				yield return new WaitForSeconds(Time.deltaTime);
			}


			//textMesh.gameObject.SetActive(false);
			parentSpawnPool.Despawn(gameObject);
		}

		/*void OnDrawGizmos()
		{
			Debug.Log("Scene");
			textMesh.GetComponent<MeshRenderer>().sortingLayerName = "Particles";
		}*/
	}
}
