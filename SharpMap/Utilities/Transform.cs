// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Text;

using GeoAPI.Geometries;

namespace SharpMap.Utilities
{
	/// <summary>
	/// Class for transforming between world and image coordinate
	/// </summary>
	public class Transform
	{
		/// <summary>
		/// Transforms from world coordinate system (WCS) to image coordinates
		/// NOTE: This method DOES NOT take the MapTransform property into account (use SharpMap.Map.MapToWorld instead)
		/// </summary>
		/// <param name="p">Point in WCS</param>
		/// <param name="map">Map reference</param>
		/// <returns>Point in image coordinates</returns>
		public static System.Drawing.PointF WorldtoMap(ICoordinate p, SharpMap.Map map)
		{
			//if (map.MapTransform != null && !map.MapTransform.IsIdentity)
			//	map.MapTransform.TransformPoints(new System.Drawing.PointF[] { p });
			System.Drawing.PointF result = new System.Drawing.Point();
			double Height = (map.Zoom * map.Size.Height) / map.Size.Width;
			double left = map.Center.X - map.Zoom*0.5;
			double top = map.Center.Y + Height * 0.5 * map.PixelAspectRatio;
			result.X = (float)((p.X - left) / map.PixelWidth);
			result.Y = (float)((top - p.Y) / map.PixelHeight);
			return result;
		}

		/// <summary>
		/// Transforms from image coordinates to world coordinate system (WCS).
		/// NOTE: This method DOES NOT take the MapTransform property into account (use SharpMap.Map.MapToWorld instead)
		/// </summary>
		/// <param name="p">Point in image coordinate system</param>
		/// <param name="map">Map reference</param>
		/// <returns>Point in WCS</returns>
		public static ICoordinate MapToWorld(System.Drawing.PointF p, SharpMap.Map map)
		{
			//if (this.MapTransform != null && !this.MapTransform.IsIdentity)
			//{
			//    System.Drawing.PointF[] p2 = new System.Drawing.PointF[] { p };
			//    this.MapTransform.TransformPoints(new System.Drawing.PointF[] { p });
			//    this.MapTransformInverted.TransformPoints(p2);
			//    return Utilities.Transform.MapToWorld(p2[0], this);
			//}
			//else 
			IEnvelope env = map.Envelope;
			return SharpMap.Converters.Geometries.GeometryFactory.CreateCoordinate(env.MinX + p.X * map.PixelWidth,
					env.MaxY - p.Y * map.PixelHeight);
		}
		
		
		#region Transform Methods
		/// <summary>
		/// Transforms the point to image coordinates, based on the map
		/// </summary>
		/// <param name="map">Map to base coordinates on</param>
		/// <returns>point in image coordinates</returns>
		public static System.Drawing.PointF TransformToImage(IPoint p, Map map)
		{
			return SharpMap.Utilities.Transform.WorldtoMap(p.Coordinate, map);
		}	
		
		/// <summary>
		/// Transforms the linestring to image coordinates, based on the map
		/// </summary>
		/// <param name="map">Map to base coordinates on</param>
		/// <returns>Linestring in image coordinates</returns>
		public static System.Drawing.PointF[] TransformToImage(ILineString line, Map map)
		{
			System.Drawing.PointF[] v = new System.Drawing.PointF[line.Coordinates.Length];
			for (int i = 0; i < line.Coordinates.Length; i++)
				v[i] = SharpMap.Utilities.Transform.WorldtoMap(line.Coordinates[i], map);
			return v;
		}	
		
		/// Transforms the polygon to image coordinates, based on the map
		/// </summary>
		/// <param name="map">Map to base coordinates on</param>
		/// <returns>Polygon in image coordinates</returns>
		public static System.Drawing.PointF[] TransformToImage(IPolygon poly, SharpMap.Map map)
		{

			int vertices = poly.Shell.Coordinates.Length;
			for (int i = 0; i < poly.Holes.Length;i++)
				vertices += poly.Holes[i].Coordinates.Length;

			System.Drawing.PointF[] v = new System.Drawing.PointF[vertices];
			for (int i = 0; i < poly.Shell.Coordinates.Length; i++)
				v[i] = SharpMap.Utilities.Transform.WorldtoMap(poly.Shell.Coordinates[i], map);
			int j = poly.Shell.Coordinates.Length;
			for (int k = 0; k < poly.Holes.Length;k++)
			{
				for (int i = 0; i < poly.Holes[k].Coordinates.Length; i++)
					v[j + i] = SharpMap.Utilities.Transform.WorldtoMap(poly.Holes[k].Coordinates[i], map);
				j += poly.Holes[k].Coordinates.Length;
			}
			return v;
		}

		
		#endregion
		
	}
}
