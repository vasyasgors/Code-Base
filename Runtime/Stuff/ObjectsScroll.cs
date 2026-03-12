using UnityEngine;
using UnityEngine.Events;
namespace CodeBase
{

	public class ObjectsScroll : MonoBehaviour
	{
		public event UnityAction MoveStarted;
		public event UnityAction MoveFinished;


		[SerializeField] protected Transform scrolledTransform;
		[SerializeField] protected float pageSize;
		[SerializeField] protected int pagesCount;
		[SerializeField] protected float movementSpeed;

		private Vector3 targetPosition;
		private int currentIndexPage;

		public int CurrentPageIndex => currentIndexPage;
		public int PagesCount => pagesCount;

		private void Awake()
		{
			targetPosition = scrolledTransform.position;
		}

		private void Update()
		{
			scrolledTransform.localPosition = Vector3.MoveTowards(scrolledTransform.localPosition, targetPosition, movementSpeed * Time.deltaTime);

			if (scrolledTransform.localPosition == targetPosition)
			{
				currentIndexPage = Mathf.Abs((int)(scrolledTransform.localPosition.x / pageSize));

				MoveFinished?.Invoke();

				enabled = false;
			}
		}

		public void MoveNext()
		{
			if (targetPosition.x > (pagesCount - 1) * -pageSize)
			{
				targetPosition.x -= pageSize;
				enabled = true;
				MoveStarted?.Invoke();
			}
		}

		public void MovePrevious()
		{
			if (targetPosition.x < 0)
			{
				targetPosition.x += pageSize;
				enabled = true;
				MoveStarted?.Invoke();
			}
		}

		public void ScrollTo(int index)
		{
			if (index < 0 || index >= pagesCount) return;

			targetPosition = scrolledTransform.position;

			targetPosition.x += -index * pageSize;
			scrolledTransform.position = targetPosition;
			currentIndexPage = index;
		}
	}
}