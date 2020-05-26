﻿using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	[SerializeField]
	Transform ground = default;

	[SerializeField]
	GameTile tilePrefab = default;

	[SerializeField]
	Texture2D gridTexture = default;

	Vector2Int size;

	GameTile[] tiles;

	Queue<GameTile> searchFrontier = new Queue<GameTile>();

	GameTileContentFactory contentFactory;

	bool showGrid, showPaths;

	public bool ShowGrid {
		get => showGrid;
		set {
			showGrid = value;
			Material m = ground.GetComponent<MeshRenderer>().material;
			if (showGrid) {
				m.mainTexture = gridTexture;
				m.SetTextureScale("_MainTex", size);
			}
			else {
				m.mainTexture = null;
			}
		}
	}
	public GameTile[] Tiles{
		get {
			return this.tiles;
		}
	}

	public bool ShowPaths {
		get => showPaths;
		set {
			showPaths = value;
			if (showPaths) {
				foreach (GameTile tile in tiles) {
					tile.ShowPath();
				}
			}
			else {
				foreach (GameTile tile in tiles) {
					tile.HidePath();
				}
			}
		}
	}

	public void Initialize (
		Vector2Int size, GameTileContentFactory contentFactory
	) {
		this.size = size;
		this.contentFactory = contentFactory;
		ground.localScale = new Vector3(size.x, size.y, 1f);

		Vector2 offset = new Vector2(
			(size.x - 1) * 0.5f, (size.y - 1) * 0.5f
		);
		tiles = new GameTile[size.x * size.y];
		for (int i = 0, y = 0; y < size.y; y++) {
			for (int x = 0; x < size.x; x++, i++) {
				GameTile tile = tiles[i] = Instantiate(tilePrefab);
				tile.transform.SetParent(transform, false);
				tile.transform.localPosition = new Vector3(
					x - offset.x, 0f, y - offset.y
				);

				if (x > 0) {
					GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
				}
				if (y > 0) {
					GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
				}

				tile.IsAlternative = (x & 1) == 0;
				if ((y & 1) == 0) {
					tile.IsAlternative = !tile.IsAlternative;
				}

				tile.Content = contentFactory.Get(GameTileContentType.Empty);
			}
		}
	}

	

	// public void ToggleWall (GameTile tile) {
	// 	if (tile.Content.Type == GameTileContentType.Wall) {
	// 		tile.Content = contentFactory.Get(GameTileContentType.Empty);
	
	// 	}
	// 	else if (tile.Content.Type == GameTileContentType.Empty) {
	// 		tile.Content = contentFactory.Get(GameTileContentType.Wall);
	// 	}
	// }


	public void ToggleWall (GameTile tile,GameTileContentType type) {
		Debug.Log("ToggleWall" + type);
		Debug.Log("tile.Content.Type");		
		
		if (tile.Content.Type == type) {
			tile.Content = contentFactory.Get(GameTileContentType.Empty);
	
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(type);
		}
	}

	public GameTile GetTile (Ray ray) {
		if (Physics.Raycast(ray, out RaycastHit hit)) {
			int x = (int)(hit.point.x + size.x * 0.5f);
			int y = (int)(hit.point.z + size.y * 0.5f);
			if (x >= 0 && x < size.x && y >= 0 && y < size.y) {
				return tiles[x + y * size.x];
			}
		}
		return null;
	}
}