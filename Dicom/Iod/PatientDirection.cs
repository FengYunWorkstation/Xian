#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Iod
{
	/// <summary>
	/// Represents a cardinal direction in the patient coordinate system.
	/// </summary>
	public sealed class PatientDirection
	{
		public enum Component
		{
			Primary = 0,
			Secondary = 1,
			Tertiary = 2
		}

		public const char UnspecifiedCode = 'X';
		public const char LeftCode = 'L';
		public const char RightCode = 'R';
		public const char AnteriorCode = 'A';
		public const char PosteriorCode = 'P';
		public const char HeadCode = 'H';
		public const char FootCode = 'F';
		public const string QuadrupedLeftCode = "LE";
		public const string QuadrupedRightCode = "RT";
		public const string QuadrupedDorsalCode = "D";
		public const string QuadrupedVentralCode = "V";
		public const string QuadrupedCranialCode = "CR";
		public const string QuadrupedCaudalCode = "CD";
		public const string QuadrupedRostralCode = "R";
		public const string QuadrupedMedialCode = "M";
		public const string QuadrupedLateralCode = "L";
		public const string QuadrupedProximalCode = "PR";
		public const string QuadrupedDistalCode = "DI";
		public const string QuadrupedPalmarCode = "PA";
		public const string QuadrupedPlantarCode = "PL";

		public static readonly PatientDirection Empty = new PatientDirection(string.Empty, string.Empty, AnatomicalOrientationType.None);
		public static readonly PatientDirection Unspecified = new PatientDirection(UnspecifiedCode.ToString(), SR.LabelPatientDirectionUnspecified, AnatomicalOrientationType.None);
		public static readonly PatientDirection Left = new PatientDirection(LeftCode.ToString(), SR.LabelPatientDirectionLeft, AnatomicalOrientationType.Biped);
		public static readonly PatientDirection Right = new PatientDirection(RightCode.ToString(), SR.LabelPatientDirectionRight, AnatomicalOrientationType.Biped);
		public static readonly PatientDirection Posterior = new PatientDirection(PosteriorCode.ToString(), SR.LabelPatientDirectionPosterior, AnatomicalOrientationType.Biped);
		public static readonly PatientDirection Anterior = new PatientDirection(AnteriorCode.ToString(), SR.LabelPatientDirectionAnterior, AnatomicalOrientationType.Biped);
		public static readonly PatientDirection Head = new PatientDirection(HeadCode.ToString(), SR.LabelPatientDirectionHead, AnatomicalOrientationType.Biped);
		public static readonly PatientDirection Foot = new PatientDirection(FootCode.ToString(), SR.LabelPatientDirectionFoot, AnatomicalOrientationType.Biped);
		public static readonly PatientDirection QuadrupedLeft = new PatientDirection(QuadrupedLeftCode, SR.LabelPatientDirectionLeft, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedRight = new PatientDirection(QuadrupedRightCode, SR.LabelPatientDirectionRight, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedDorsal = new PatientDirection(QuadrupedDorsalCode, SR.LabelPatientDirectionDorsal, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedVentral = new PatientDirection(QuadrupedVentralCode, SR.LabelPatientDirectionVentral, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedCranial = new PatientDirection(QuadrupedCranialCode, SR.LabelPatientDirectionCranial, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedCaudal = new PatientDirection(QuadrupedCaudalCode, SR.LabelPatientDirectionCaudal, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedRostral = new PatientDirection(QuadrupedRostralCode, SR.LabelPatientDirectionRostral, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedMedial = new PatientDirection(QuadrupedMedialCode, SR.LabelPatientDirectionMedial, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedLateral = new PatientDirection(QuadrupedLateralCode, SR.LabelPatientDirectionLateral, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedProximal = new PatientDirection(QuadrupedProximalCode, SR.LabelPatientDirectionProximal, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedDistal = new PatientDirection(QuadrupedDistalCode, SR.LabelPatientDirectionDistal, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedPalmar = new PatientDirection(QuadrupedPalmarCode, SR.LabelPatientDirectionPalmar, AnatomicalOrientationType.Quadruped);
		public static readonly PatientDirection QuadrupedPlantar = new PatientDirection(QuadrupedPlantarCode, SR.LabelPatientDirectionPlantar, AnatomicalOrientationType.Quadruped);

		/// <summary>
		/// Enumeration of directions available in a biped anatomical orientation system.
		/// </summary>
		public static IEnumerable<PatientDirection> BipedDirections
		{
			get
			{
				yield return Left;
				yield return Right;
				yield return Head;
				yield return Foot;
				yield return Posterior;
				yield return Anterior;
			}
		}

		/// <summary>
		/// Enumeration of directions available in a quadruped anatomical orientation system.
		/// </summary>
		public static IEnumerable<PatientDirection> QuadrupedDirections
		{
			get
			{
				yield return QuadrupedLeft;
				yield return QuadrupedRight;
				yield return QuadrupedDorsal;
				yield return QuadrupedVentral;
				yield return QuadrupedCranial;
				yield return QuadrupedCaudal;
				yield return QuadrupedRostral;
				yield return QuadrupedMedial;
				yield return QuadrupedLateral;
				yield return QuadrupedProximal;
				yield return QuadrupedDistal;
				yield return QuadrupedPalmar;
				yield return QuadrupedPlantar;
			}
		}

		private readonly AnatomicalOrientationType _anatomicalOrientationType;
		private readonly PatientDirection _primaryComponent;
		private readonly PatientDirection _secondaryComponent;
		private readonly PatientDirection _tertiaryComponent;
		private readonly string _code;
		private readonly string _description;
		private readonly bool _isValid;
		private readonly int _componentCount;

		/// <summary>
		/// Initializes a standard component direction.
		/// </summary>
		private PatientDirection(string code, string description, AnatomicalOrientationType anatomicalOrientationType)
		{
			_primaryComponent = this;
			_secondaryComponent = null;
			_tertiaryComponent = null;
			_isValid = true;
			_code = code;
			_description = description;
			_componentCount = 1;
			_anatomicalOrientationType = anatomicalOrientationType;
		}

		public PatientDirection(PatientDirection patientDirection)
			: this(patientDirection.Code, patientDirection.AnatomicalOrientationType) {}

		public PatientDirection(string code)
			: this(code, AnatomicalOrientationType.Biped) {}

		public PatientDirection(string code, AnatomicalOrientationType anatomicalOrientationType)
		{
			PatientDirection[] components;
			if (anatomicalOrientationType == AnatomicalOrientationType.Quadruped)
			{
				components = ParseQuadrupedDirection(code);
			}
			else
			{
				anatomicalOrientationType = AnatomicalOrientationType.Biped; // assume orientation type is BIPED
				components = ParseBipedDirection(code);
			}

			// set the parsed components
			_primaryComponent = components.Length > 0 ? components[0] : null;
			_secondaryComponent = components.Length > 1 ? components[1] : null;
			_tertiaryComponent = components.Length > 2 ? components[2] : null;

			_isValid = components.Length > 0;
			_code = code ?? string.Empty;
			_description = string.Join(SR.LabelPatientDirectionSeparator, CollectionUtils.Map<PatientDirection, string>(components, d => d.Description).ToArray());
			_componentCount = components.Length;

			// consider orientation type to be NONE if direction is empty or unspecified (and not just empty because of parse error)
			_anatomicalOrientationType = string.IsNullOrEmpty(code) || Primary.IsUnspecified ? AnatomicalOrientationType.None : anatomicalOrientationType;
		}

		/// <summary>
		/// Gets the anatomical orientation system used by this direction.
		/// </summary>
		public AnatomicalOrientationType AnatomicalOrientationType
		{
			get { return _anatomicalOrientationType; }
		}

		/// <summary>
		/// Gets the direction code string.
		/// </summary>
		public string Code
		{
			get { return _code; }
		}

	    // TODO (CR Mar 2012): Add a "short description" which could essentially be used in places where we show/use directional markers?

		/// <summary>
		/// Gets a textual description of the direction.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		/// <summary>
		/// Gets the number of components in this direction.
		/// </summary>
		public int ComponentCount
		{
			get { return _componentCount; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this direction is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(_code); }
		}

		/// <summary>
		/// Gets a value indicating whether or not this direction represents the <see cref="Unspecified"/> direction.
		/// </summary>
		public bool IsUnspecified
		{
			get { return _code == "X"; }
		}

		/// <summary>
		/// Gets a value indicating whether or not this direction is a valid non-trivial direction.
		/// </summary>
		public bool IsValid
		{
			get { return _isValid; }
		}

		/// <summary>
		/// Gets the specified component of this direction.
		/// </summary>
		/// <param name="component">The component to be returned.</param>
		public PatientDirection this[Component component]
		{
			get
			{
				switch (component)
				{
					case Component.Primary:
						return Primary;
					case Component.Secondary:
						return Secondary;
					case Component.Tertiary:
						return Tertiary;
					default:
						throw new ArgumentOutOfRangeException("component");
				}
			}
		}

		/// <summary>
		/// Gets the primary component of this direction.
		/// </summary>
		public PatientDirection Primary
		{
			get { return _primaryComponent ?? Empty; }
		}

		/// <summary>
		/// Gets the secondary component of this direction.
		/// </summary>
		public PatientDirection Secondary
		{
			get { return _secondaryComponent ?? Empty; }
		}

		/// <summary>
		/// Gets the tertiary component of this direction.
		/// </summary>
		public PatientDirection Tertiary
		{
			get { return _tertiaryComponent ?? Empty; }
		}

		/// <summary>
		/// Checks whether or not <paramref name="obj"/> is an equivalent <see cref="PatientDirection"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is PatientDirection)
				return _code == ((PatientDirection) obj).Code && _anatomicalOrientationType == ((PatientDirection) obj).AnatomicalOrientationType;
			return false;
		}

		/// <summary>
		/// Gets the direction opposite to the current direction.
		/// </summary>
		/// <remarks>
		/// Opposing directions are only available if <see cref="AnatomicalOrientationType"/> is <see cref="Iod.AnatomicalOrientationType.Biped"/>.
		/// </remarks>
		public PatientDirection OpposingDirection
		{
			get
			{
				if (AnatomicalOrientationType == AnatomicalOrientationType.Quadruped)
					return Unspecified;

				var opposing = string.Empty;
				opposing += GetOpposingDirection(Primary.Code);
				opposing += GetOpposingDirection(Secondary.Code);
				opposing += GetOpposingDirection(Tertiary.Code);
				return new PatientDirection(opposing, AnatomicalOrientationType);
			}
		}

		public override int GetHashCode()
		{
			return 0x78df6aF ^ Code.GetHashCode() ^ AnatomicalOrientationType.GetHashCode();
		}

		public override string ToString()
		{
			return Code;
		}

		public static PatientDirection operator +(PatientDirection d1, PatientDirection d2)
		{
			if (d1.AnatomicalOrientationType != d2.AnatomicalOrientationType && d1.AnatomicalOrientationType != AnatomicalOrientationType.None && d2.AnatomicalOrientationType != AnatomicalOrientationType.None)
				throw new ArgumentException("Concatenation of different anatomical orientation directions is not allowed.");
			return new PatientDirection(d1.Code + d2.Code, d1.AnatomicalOrientationType);
		}

		public static PatientDirection operator -(PatientDirection direction)
		{
			return direction.OpposingDirection;
		}

		public static bool operator ==(PatientDirection d1, PatientDirection d2)
		{
			return Equals(d1, d2);
		}

		public static bool operator !=(PatientDirection d1, PatientDirection d2)
		{
			return !Equals(d1, d2);
		}

		public static implicit operator string(PatientDirection direction)
		{
			return direction.ToString();
		}

		public static implicit operator PatientDirection(ImageOrientationPatient.Directions direction)
		{
			switch (direction)
			{
				case ImageOrientationPatient.Directions.Left:
					return Left;
				case ImageOrientationPatient.Directions.Right:
					return Right;
				case ImageOrientationPatient.Directions.Anterior:
					return Anterior;
				case ImageOrientationPatient.Directions.Posterior:
					return Posterior;
				case ImageOrientationPatient.Directions.Head:
					return Head;
				case ImageOrientationPatient.Directions.Foot:
					return Foot;
			}

			return Empty;
		}

		private static string GetOpposingDirection(string component)
		{
			if (string.IsNullOrEmpty(component))
				return string.Empty;

			switch (component[0])
			{
				case LeftCode:
					return RightCode.ToString();
				case RightCode:
					return LeftCode.ToString();
				case AnteriorCode:
					return PosteriorCode.ToString();
				case PosteriorCode:
					return AnteriorCode.ToString();
				case HeadCode:
					return FootCode.ToString();
				case FootCode:
					return HeadCode.ToString();
			}

			return UnspecifiedCode.ToString();
		}

		private static PatientDirection[] ParseBipedDirection(string code)
		{
			if (string.IsNullOrEmpty(code))
				return new PatientDirection[0];

			if (code == "X")
				return new[] {Unspecified};

			// if there are more than 3 components, input is invalid
			if (code.Length > 3)
				return new PatientDirection[0];

			var components = new List<PatientDirection>(3);
			for (int charPos = 0; charPos < code.Length; charPos++)
			{
				PatientDirection component;
				switch (code[charPos])
				{
					case 'H':
						component = Head;
						break;
					case 'F':
						component = Foot;
						break;
					case 'L':
						component = Left;
						break;
					case 'R':
						component = Right;
						break;
					case 'P':
						component = Posterior;
						break;
					case 'A':
						component = Anterior;
						break;
					default:
						return new PatientDirection[0];
				}
				components.Add(component);
			}

			// verify that each component is unique and not along the same direction
			var normalized = code.Replace(RightCode, LeftCode);
			normalized = normalized.Replace(AnteriorCode, PosteriorCode);
			normalized = normalized.Replace(FootCode, HeadCode);
			if ((CollectionUtils.Unique(normalized).Count != code.Length))
				return new PatientDirection[0];

			return components.ToArray();
		}

		private static PatientDirection[] ParseQuadrupedDirection(string code)
		{
			if (string.IsNullOrEmpty(code))
				return new PatientDirection[0];

			if (code == "X")
				return new[] {Unspecified};

			var components = new List<PatientDirection>(3);
			for (int charPos = 0; charPos < code.Length; charPos++)
			{
				// if we've already parsed 3 components and there's still more, input is invalid
				if (components.Count >= 3)
					return new PatientDirection[0];

				PatientDirection component;

				var nextChar = charPos + 1 < code.Length ? code[charPos + 1] : '\0';
				if (code[charPos] == 'L' && nextChar == 'E')
				{
					component = QuadrupedLeft;
					charPos++;
				}
				else if (code[charPos] == 'R' && nextChar == 'T')
				{
					component = QuadrupedRight;
					charPos++;
				}
				else if (code[charPos] == 'C' && nextChar == 'R')
				{
					component = QuadrupedCranial;
					charPos++;
				}
				else if (code[charPos] == 'C' && nextChar == 'D')
				{
					component = QuadrupedCaudal;
					charPos++;
				}
				else if (code[charPos] == 'D' && nextChar == 'I')
				{
					component = QuadrupedDistal;
					charPos++;
				}
				else if (code[charPos] == 'P' && nextChar == 'R')
				{
					component = QuadrupedProximal;
					charPos++;
				}
				else if (code[charPos] == 'P' && nextChar == 'A')
				{
					component = QuadrupedPalmar;
					charPos++;
				}
				else if (code[charPos] == 'P' && nextChar == 'L')
				{
					component = QuadrupedPlantar;
					charPos++;
				}
				else if (code[charPos] == 'R')
				{
					component = QuadrupedRostral;
				}
				else if (code[charPos] == 'L')
				{
					component = QuadrupedLateral;
				}
				else if (code[charPos] == 'D')
				{
					component = QuadrupedDorsal;
				}
				else if (code[charPos] == 'V')
				{
					component = QuadrupedVentral;
				}
				else if (code[charPos] == 'M')
				{
					component = QuadrupedMedial;
				}
				else // unknown component
				{
					return new PatientDirection[0];
				}

				components.Add(component);
			}

			return components.ToArray();
		}
	}
}