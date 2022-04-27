using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeoGXP.NeoGXP.Managers;

//------------------------------------------------------------------------------------------------------------------------
//														CollisionManager
//------------------------------------------------------------------------------------------------------------------------
public class CollisionManager
{
	/// <summary>
	/// Set this to false if you want to be able to remove game objects from the game during OnCollision (=the old, unsafe default behavior).
	/// </summary>
	public const bool SAFE_COLLISION_LOOP = true;

	/// <summary>
	/// Set this to true if you only want to include trigger colliders in OnCollision (=more efficient).
	/// </summary>
	public const bool TRIGGERS_ONLY_ON_COLLISION = false;

	private delegate void CollisionDelegate(GameObject gameObject);

	//------------------------------------------------------------------------------------------------------------------------
	//														ColliderInfo
	//------------------------------------------------------------------------------------------------------------------------
	private struct ColliderInfo {
		public GameObject gameObject;
		public CollisionDelegate onCollision;

		//------------------------------------------------------------------------------------------------------------------------
		//														ColliderInfo()
		//------------------------------------------------------------------------------------------------------------------------
		public ColliderInfo(GameObject gameObject, CollisionDelegate onCollision) {
			this.gameObject = gameObject;
			this.onCollision = onCollision;
		}
	}

	private List<GameObject> colliderList = new();
	private List<ColliderInfo> activeColliderList = new();
	private Dictionary<GameObject, ColliderInfo> _collisionReferences = new();

	private bool collisionLoopActive = false;

	//------------------------------------------------------------------------------------------------------------------------
	//														CollisionManager()
	//------------------------------------------------------------------------------------------------------------------------
	public CollisionManager ()
	{
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Step()
	//------------------------------------------------------------------------------------------------------------------------
	public void Step() {
		collisionLoopActive = SAFE_COLLISION_LOOP;
		for (int i=activeColliderList.Count-1; i>= 0; i--) {
			ColliderInfo info = activeColliderList[i];
			for (int j=colliderList.Count-1; j>=0; j--) {
				if (j >= colliderList.Count) continue; //fix for removal in loop
				GameObject other = colliderList[j];
				if (other.collider == null || !(other.collider.isTrigger || !TRIGGERS_ONLY_ON_COLLISION)) continue;
				if (info.gameObject == other) continue;
				if (!info.gameObject.HitTest(other)) continue;
				info.onCollision?.Invoke(other);
			}
		}
		collisionLoopActive = false;
	}

	//------------------------------------------------------------------------------------------------------------------------
	//												 GetCurrentCollisions()
	//------------------------------------------------------------------------------------------------------------------------
	public GameObject[] GetCurrentCollisions (GameObject gameObject, bool includeTriggers=true, bool includeSolid=true)
	{
		List<GameObject> list = new();
		for (int j=colliderList.Count-1; j>=0; j--) {
			if (j >= colliderList.Count) continue; //fix for removal in loop
			GameObject other = colliderList[j];
			if (other.collider == null || (other.collider.isTrigger && !includeTriggers) || (!other.collider.isTrigger && !includeSolid)) continue;
			if (gameObject == other) continue;
			if (gameObject.HitTest(other)) {
				list.Add(other);
			}
		}
		return list.ToArray();
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Add()
	//------------------------------------------------------------------------------------------------------------------------
	public void Add(GameObject gameObject) {
		if (collisionLoopActive) {
			throw new Exception ("Cannot call AddChild for gameobjects during OnCollision - use LateAddChild instead.");
		}
		if (gameObject.collider != null && !colliderList.Contains (gameObject)) {
			colliderList.Add(gameObject);
		}

		MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

		if (info != null) {

			CollisionDelegate onCollision = (CollisionDelegate)Delegate.CreateDelegate(typeof(CollisionDelegate), gameObject, info, false);
			if (onCollision == null || _collisionReferences.ContainsKey(gameObject)) return;
			ColliderInfo colliderInfo = new(gameObject, onCollision);
			_collisionReferences[gameObject] = colliderInfo;
			activeColliderList.Add(colliderInfo);

		} else {
			ValidateCase(gameObject);
		}
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														validateCase()
	//------------------------------------------------------------------------------------------------------------------------
	private static void ValidateCase(GameObject gameObject) {
		MethodInfo info = gameObject.GetType().GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
		if (info != null) {
			throw new Exception("'OnCollision' function was not bound. Please check its case (capital O?)");
		}
	}

	//------------------------------------------------------------------------------------------------------------------------
	//														Remove()
	//------------------------------------------------------------------------------------------------------------------------
	public void Remove(GameObject gameObject) {
		if (collisionLoopActive) {
			throw new Exception ("Cannot destroy or remove gameobjects during OnCollision - use LateDestroy or LateRemove instead.");
		}
		colliderList.Remove(gameObject);
		if (!_collisionReferences.ContainsKey(gameObject)) return;
		ColliderInfo colliderInfo = _collisionReferences[gameObject];
		activeColliderList.Remove(colliderInfo);
		_collisionReferences.Remove(gameObject);
	}

	public string GetDiagnostics() {
		string output = "";
		output += "Number of colliders: " + colliderList.Count+'\n';
		output += "Number of active colliders: " + activeColliderList.Count+'\n';
		return output;
	}
}
