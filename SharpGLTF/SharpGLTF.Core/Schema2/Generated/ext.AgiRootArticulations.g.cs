// <auto-generated/>

//------------------------------------------------------------------------------------------------
//      This file has been programatically generated; DON´T EDIT!
//------------------------------------------------------------------------------------------------

#pragma warning disable SA1001
#pragma warning disable SA1027
#pragma warning disable SA1028
#pragma warning disable SA1121
#pragma warning disable SA1205
#pragma warning disable SA1309
#pragma warning disable SA1402
#pragma warning disable SA1505
#pragma warning disable SA1507
#pragma warning disable SA1508
#pragma warning disable SA1652

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Text.Json;

namespace SharpGLTF.Schema2
{
	using Collections;

	/// <summary>
	/// The type of motion applied by this articulation stage.
	/// </summary>
	public enum AgiArticulationTransformType
	{
		xTranslate,
		yTranslate,
		zTranslate,
		xRotate,
		yRotate,
		zRotate,
		xScale,
		yScale,
		zScale,
		uniformScale,
	}


	/// <summary>
	/// One stage of a model articulation definition.
	/// </summary>
	partial class AgiArticulationStage : ExtraProperties
	{
	
		private Double _initialValue;
		
		private Double _maximumValue;
		
		private Double _minimumValue;
		
		private String _name;
		
		private AgiArticulationTransformType _type;
		
	
		protected override void SerializeProperties(Utf8JsonWriter writer)
		{
			base.SerializeProperties(writer);
			SerializeProperty(writer, "initialValue", _initialValue);
			SerializeProperty(writer, "maximumValue", _maximumValue);
			SerializeProperty(writer, "minimumValue", _minimumValue);
			SerializeProperty(writer, "name", _name);
			SerializePropertyEnumSymbol<AgiArticulationTransformType>(writer, "type", _type);
		}
	
		protected override void DeserializeProperty(string jsonPropertyName, ref Utf8JsonReader reader)
		{
			switch (jsonPropertyName)
			{
				case "initialValue": _initialValue = DeserializePropertyValue<Double>(ref reader); break;
				case "maximumValue": _maximumValue = DeserializePropertyValue<Double>(ref reader); break;
				case "minimumValue": _minimumValue = DeserializePropertyValue<Double>(ref reader); break;
				case "name": _name = DeserializePropertyValue<String>(ref reader); break;
				case "type": _type = DeserializePropertyValue<AgiArticulationTransformType>(ref reader); break;
				default: base.DeserializeProperty(jsonPropertyName,ref reader); break;
			}
		}
	
	}

	/// <summary>
	/// A model articulation definition.
	/// </summary>
	partial class AgiArticulation : ExtraProperties
	{
	
		private String _name;
		
		private Vector3? _pointingVector;
		
		private const int _stagesMinItems = 1;
		private ChildrenCollection<AgiArticulationStage,AgiArticulation> _stages;
		
	
		protected override void SerializeProperties(Utf8JsonWriter writer)
		{
			base.SerializeProperties(writer);
			SerializeProperty(writer, "name", _name);
			SerializeProperty(writer, "pointingVector", _pointingVector);
			SerializeProperty(writer, "stages", _stages, _stagesMinItems);
		}
	
		protected override void DeserializeProperty(string jsonPropertyName, ref Utf8JsonReader reader)
		{
			switch (jsonPropertyName)
			{
				case "name": _name = DeserializePropertyValue<String>(ref reader); break;
				case "pointingVector": _pointingVector = DeserializePropertyValue<Vector3?>(ref reader); break;
				case "stages": DeserializePropertyList<AgiArticulationStage>(ref reader, _stages); break;
				default: base.DeserializeProperty(jsonPropertyName,ref reader); break;
			}
		}
	
	}

	/// <summary>
	/// glTF Extension that defines metadata for applying external analysis or effects to a model.
	/// </summary>
	partial class AgiRootArticulations : ExtraProperties
	{
	
		private const int _articulationsMinItems = 1;
		private ChildrenCollection<AgiArticulation,AgiRootArticulations> _articulations;
		
	
		protected override void SerializeProperties(Utf8JsonWriter writer)
		{
			base.SerializeProperties(writer);
			SerializeProperty(writer, "articulations", _articulations, _articulationsMinItems);
		}
	
		protected override void DeserializeProperty(string jsonPropertyName, ref Utf8JsonReader reader)
		{
			switch (jsonPropertyName)
			{
				case "articulations": DeserializePropertyList<AgiArticulation>(ref reader, _articulations); break;
				default: base.DeserializeProperty(jsonPropertyName,ref reader); break;
			}
		}
	
	}

}
