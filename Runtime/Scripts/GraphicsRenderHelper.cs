using System;
using System.Collections.Generic;
using System.Linq;
using Knifest.UniTools.Extensions;
using UnityEngine;

namespace Knifest.UniTools
{
    public class GraphicsRenderHelper
    {
        public RenderParams Params;
        
        private Dictionary<MeshRenderArgs, List<Matrix4x4>> _renderMatrices = new();
        private Dictionary<MeshRenderArgs, Dictionary<string, List<float>>> _renderFloats = new();
        private Dictionary<MeshRenderArgs, Dictionary<string, List<Color>>> _renderColors = new();
        
        public void Init()
        {
            Params.matProps = new MaterialPropertyBlock();
        }
        
        public void Clear()
        {
            foreach (var matrices in _renderMatrices.Values) matrices.Clear();

            foreach (var properties in _renderFloats.Values)
            foreach (var value in properties.Values)
                value.Clear();
            
            foreach (var properties in _renderColors.Values)
            foreach (var value in properties.Values)
                value.Clear();
        }
        
        public void AddMatrix(IReadOnlyList<Material> materials, Mesh mesh, Matrix4x4 matrix)
        {
            for (var i = 0; i < materials.Count; i++) AddMatrix(materials[i], mesh, matrix, i);
        }

        public void AddMatrix(Material material, Mesh mesh, Matrix4x4 matrix, int submeshIndex = 0)
        {
            var key = new MeshRenderArgs(mesh, material, submeshIndex);
            if (_renderMatrices.TryGetValue(key, out var matrices))
                matrices.Add(matrix);
            else
                _renderMatrices.Add(key, new List<Matrix4x4> { matrix });
        }
        
        public void AddFloat(Material material, Mesh mesh, string floatName, float value = 0, int submeshIndex = 0)
        {
            var key = new MeshRenderArgs(mesh, material, submeshIndex);
            if (!_renderFloats.TryGetValue(key, out var floatProperties))
            {
                floatProperties = new Dictionary<string, List<float>>();
                _renderFloats.Add(key, floatProperties);
            }

            if (floatProperties.TryGetValue(floatName, out var floats))
                floats.Add(value);
            else
                floatProperties.Add(floatName, new List<float> { value });
        }
        
        public void AddColor(Material material, Mesh mesh, string parameterName, Color value = default, int submeshIndex = 0)
        {
            var key = new MeshRenderArgs(mesh, material, submeshIndex);
            if (!_renderColors.TryGetValue(key, out var properties))
            {
                properties = new Dictionary<string, List<Color>>();
                _renderColors.Add(key, properties);
            }

            if (properties.TryGetValue(parameterName, out var values))
                values.Add(value);
            else
                properties.Add(parameterName, new List<Color> { value });
        }
        
        public void Render()
        {
            foreach (var renderMatrix in _renderMatrices)
                Render(renderMatrix.Key, renderMatrix.Value);
        }
        
        private void Render(MeshRenderArgs args, List<Matrix4x4> matrices)
        {
            if (!matrices.Any()) return;

            bool hasProps = false;
            
            if (_renderFloats.TryGetValue(args, out var floatProperties))
            {
                foreach (var floatProperty in floatProperties.Where(floatProperty => floatProperty.Value.Any()))
                    Params.matProps.SetFloatArray(floatProperty.Key, floatProperty.Value);

                hasProps = true;
            }
            
            if (_renderColors.TryGetValue(args, out var colorProperties))
            {
                foreach (var property in colorProperties.Where(property => property.Value.Any()))
                    Params.matProps.SetVectorArray(property.Key, property.Value.Select(c=>c.ToVector4()).ToList());

                hasProps = true;
            }
                
            Params.material = args.Material;
            Graphics.RenderMeshInstanced(Params, args.Mesh, args.SubmeshIndex, matrices);
            
            if(hasProps) Params.matProps.Clear();
        }
        
        private struct MeshRenderArgs : IEquatable<MeshRenderArgs>
        {
            public Mesh Mesh;
            public Material Material;
            public int SubmeshIndex;

            public MeshRenderArgs(Mesh mesh, Material material, int submeshIndex = 0)
            {
                Mesh = mesh;
                Material = material;
                SubmeshIndex = submeshIndex;
            }

            public bool Equals(MeshRenderArgs other)
            {
                return Equals(Mesh, other.Mesh) && Equals(Material, other.Material) &&
                       Equals(SubmeshIndex, other.SubmeshIndex);
            }

            public override bool Equals(object obj)
            {
                return obj is MeshRenderArgs other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Mesh, Material, SubmeshIndex);
            }
        }
    }
}