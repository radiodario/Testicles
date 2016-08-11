using UnityEngine;
using System.Collections;

public class HexagonDrawer : MonoBehaviour
{

	Vector3 positions;

	public static Vector2 NumberOfForces(int width, int height, int scale) {
		float s = scale;
		float a = Mathf.Sqrt(3) * s / 2;

		int cols = (int) Mathf.Floor (width / a / 2);
		int rows = (int) Mathf.Floor ((height / s / 3) * 2) + 1;

		return new Vector2(cols , rows);

	}

	public static Texture2D DrawHexagonSurface(int width, int height, int scale) {

		KDTree searcher;

		float s = scale;
		float a = Mathf.Sqrt(3) * s / 2;

		int cols = (int) Mathf.Floor(width / a / 2);
		int rows = (int)Mathf.Floor ((height / s / 3) * 2) + 1;

	
		Texture2D positionTexture = new Texture2D (width, height, TextureFormat.ARGB32, false);
		RenderTexture outTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

		Vector3[] ForcePositions = new Vector3[rows * cols * 2];
		Vector3[] hexCentres = new Vector3[rows * cols];


		int idx = 0;
	

		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				Vector3 hexCentre = new Vector3 (
					                   (i % 2) * a + 2 * j * a,
					                   i * ((3 * s) / 2),
										0
				                   );

				Vector4 pos1 = new Vector3 (
					               hexCentre.x,
					               hexCentre.y + (s),
					               0
				               );
				hexCentres [j + i * cols] = hexCentre;			

				ForcePositions [idx] = pos1;

				Vector3 pos2;
				if (i % 2 == 0) {
					// top left
					pos2 = new Vector4 (
						hexCentre.x - a,
						hexCentre.y + s / 2,
						0
					);
				} else {
					// top right
					pos2 = new Vector3 (
						hexCentre.x + a,
						hexCentre.y + s / 2,
						0
					);
				}

				ForcePositions [idx + 1] = pos2;
				idx += 2;

			}
		}

		searcher = KDTree.MakeFromPoints (ForcePositions);

		Color[] colors = new Color[width*height];
		int cIdx = 0;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int forceIndex = searcher.FindNearest (new Vector3 (x, y, 0));
				colors[cIdx] = HexagonDrawer.indexToColor(forceIndex, ForcePositions.Length);
				cIdx++;
			}
		}
		positionTexture.SetPixels(colors);
		positionTexture.Apply ();

		return positionTexture;

	}

	private static Color indexToColor(int index, int numForces) {
		float red;
		float green;

		int side = (int) Mathf.Sqrt (numForces);
		red = (index % side) / (float) side;
		green = (index / side) / (float) side;

		return new Color (red, green, 0, index % 2);
	}


}

