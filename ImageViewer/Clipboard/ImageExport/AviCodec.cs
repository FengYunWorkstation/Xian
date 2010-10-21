﻿#region License (non-CC)

// This software is licensed un the Code Project Open License 1.02,
// the terms of which are listed as follows.
//
// Preamble
//
// This License governs Your use of the Work. This License is intended to allow
// developers to use the Source Code and Executable Files provided as part of the
// Work in any application in any form.
//
// The main points subject to the terms of the License are:
//
//   * Source Code and Executable Files can be used in commercial applications;
//   * Source Code and Executable Files can be redistributed; and
//   * Source Code can be modified to create derivative works.
//   * No claim of suitability, guarantee, or any warranty whatsoever is provided.
//     The software is provided "as-is".
//   * The Article(s) accompanying the Work may not be distributed or republished
//     without the Author's consent
//
// This License is entered between You, the individual or other entity reading or
// otherwise making use of the Work licensed pursuant to this License and the
// individual or other entity which offers the Work under the terms of this License
// ("Author").
//
// License
//
// THE WORK (AS DEFINED BELOW) IS PROVIDED UNDER THE TERMS OF THIS CODE PROJECT
// OPEN LICENSE ("LICENSE"). THE WORK IS PROTECTED BY COPYRIGHT AND/OR OTHER
// APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER THIS LICENSE
// OR COPYRIGHT LAW IS PROHIBITED.
//
// BY EXERCISING ANY RIGHTS TO THE WORK PROVIDED HEREIN, YOU ACCEPT AND AGREE TO BE
// BOUND BY THE TERMS OF THIS LICENSE. THE AUTHOR GRANTS YOU THE RIGHTS CONTAINED
// HEREIN IN CONSIDERATION OF YOUR ACCEPTANCE OF SUCH TERMS AND CONDITIONS. IF YOU
// DO NOT AGREE TO ACCEPT AND BE BOUND BY THE TERMS OF THIS LICENSE, YOU CANNOT
// MAKE ANY USE OF THE WORK.
//
//   1. Definitions.
//      1. "Articles" means, collectively, all articles written by Author which
//         describes how the Source Code and Executable Files for the Work may be
//         used by a user.
//      2. "Author" means the individual or entity that offers the Work under the
//         terms of this License.
//      3. "Derivative Work" means a work based upon the Work or upon the Work and
//         other pre-existing works.
//      4. "Executable Files" refer to the executables, binary files, configuration
//         and any required data files included in the Work.
//      5. "Publisher" means the provider of the website, magazine, CD-ROM, DVD or
//         other medium from or by which the Work is obtained by You.
//      6. "Source Code" refers to the collection of source code and configuration
//         files used to create the Executable Files.
//      7. "Standard Version" refers to such a Work if it has not been modified, or
//         has been modified in accordance with the consent of the Author, such
//         consent being in the full discretion of the Author.
//      8. "Work" refers to the collection of files distributed by the Publisher,
//         including the Source Code, Executable Files, binaries, data files,
//         documentation, whitepapers and the Articles.
//      9. "You" is you, an individual or entity wishing to use the Work and
//         exercise your rights under this License.
//   2. Fair Use/Fair Use Rights. Nothing in this License is intended to reduce,
//      limit, or restrict any rights arising from fair use, fair dealing, first
//      sale or other limitations on the exclusive rights of the copyright owner
//      under copyright law or other applicable laws.
//   3. License Grant. Subject to the terms and conditions of this License, the
//      Author hereby grants You a worldwide, royalty-free, non-exclusive,
//      perpetual (for the duration of the applicable copyright) license to
//      exercise the rights in the Work as stated below:
//      1. You may use the standard version of the Source Code or Executable Files
//         in Your own applications.
//      2. You may apply bug fixes, portability fixes and other modifications
//         obtained from the Public Domain
//         or from the Author. A Work modified in such a way shall still be
//         considered the standard version and will be subject to this License.
//      3. You may otherwise modify Your copy of this Work (excluding the Articles)
//         in any way to create a Derivative Work, provided that You insert a
//         prominent notice in each changed file stating how, when and where You
//         changed that file.
//      4. You may distribute the standard version of the Executable Files and
//         Source Code or Derivative Work in aggregate with other (possibly
//         commercial) programs as part of a larger (possibly commercial) software
//         distribution.
//      5. The Articles discussing the Work published in any form by the author may
//         not be distributed or republished without the Author's consent. The
//         author retains copyright to any such Articles. You may use the
//         Executable Files and Source Code pursuant to this License but you may
//         not repost or republish or otherwise distribute or make available the
//         Articles, without the prior written consent of the Author. Any
//         subroutines or modules supplied by You and linked into the Source Code
//         or Executable Files this Work shall not be considered part of this Work
//         and will not be subject to the terms of this License.
//   4. Patent License. Subject to the terms and conditions of this License, each
//      Author hereby grants to You a perpetual, worldwide, non-exclusive,
//      no-charge, royalty-free, irrevocable (except as stated in this section)
//      patent license to make, have made, use, import, and otherwise transfer the
//      Work.
//   5. Restrictions. The license granted in Section 3 above is expressly made
//      subject to and limited by the following restrictions:
//      1. You agree not to remove any of the original copyright, patent,
//         trademark, and attribution notices and associated disclaimers that may
//         appear in the Source Code or Executable Files.
//      2. You agree not to advertise or in any way imply that this Work is a
//         product of Your own.
//      3. The name of the Author may not be used to endorse or promote products
//         derived from the Work without the prior written consent of the Author.
//      4. You agree not to sell, lease, or rent any part of the Work. This does
//         not restrict you from including the Work or any part of the Work inside
//         a larger software distribution that itself is being sold. The Work by
//         itself, though, cannot be sold, leased or rented.
//      5. You may distribute the Executable Files and Source Code only under the
//         terms of this License, and You must include a copy of, or the Uniform
//         Resource Identifier for, this License with every copy of the Executable
//         Files or Source Code You distribute and ensure that anyone receiving
//         such Executable Files and Source Code agrees that the terms of this
//         License apply to such Executable Files and/or Source Code. You may not
//         offer or impose any terms on the Work that alter or restrict the terms
//         of this License or the recipients' exercise of the rights granted
//         hereunder. You may not sublicense the Work. You must keep intact all
//         notices that refer to this License and to the disclaimer of warranties.
//         You may not distribute the Executable Files or Source Code with any
//         technological measures that control access or use of the Work in a
//         manner inconsistent with the terms of this License.
//      6. You agree not to use the Work for illegal, immoral or improper purposes,
//         or on pages containing illegal, immoral or improper material. The Work
//         is subject to applicable export laws. You agree to comply with all such
//         laws and regulations that may apply to the Work after Your receipt of
//         the Work.
//   6. Representations, Warranties and Disclaimer. THIS WORK IS PROVIDED "AS IS",
//      "WHERE IS" AND "AS AVAILABLE", WITHOUT ANY EXPRESS OR IMPLIED WARRANTIES OR
//      CONDITIONS OR GUARANTEES. YOU, THE USER, ASSUME ALL RISK IN ITS USE,
//      INCLUDING COPYRIGHT INFRINGEMENT, PATENT INFRINGEMENT, SUITABILITY, ETC.
//      AUTHOR EXPRESSLY DISCLAIMS ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES OR
//      CONDITIONS, INCLUDING WITHOUT LIMITATION, WARRANTIES OR CONDITIONS OF
//      MERCHANTABILITY, MERCHANTABLE QUALITY OR FITNESS FOR A PARTICULAR PURPOSE,
//      OR ANY WARRANTY OF TITLE OR NON-INFRINGEMENT, OR THAT THE WORK (OR ANY
//      PORTION THEREOF) IS CORRECT, USEFUL, BUG-FREE OR FREE OF VIRUSES. YOU MUST
//      PASS THIS DISCLAIMER ON WHENEVER YOU DISTRIBUTE THE WORK OR DERIVATIVE
//      WORKS.
//   7. Indemnity. You agree to defend, indemnify and hold harmless the Author and
//      the Publisher from and against any claims, suits, losses, damages,
//      liabilities, costs, and expenses (including reasonable legal or attorneys'
//      fees) resulting from or relating to any use of the Work by You.
//   8. Limitation on Liability. EXCEPT TO THE EXTENT REQUIRED BY APPLICABLE LAW,
//      IN NO EVENT WILL THE AUTHOR OR THE PUBLISHER BE LIABLE TO YOU ON ANY LEGAL
//      THEORY FOR ANY SPECIAL, INCIDENTAL, CONSEQUENTIAL, PUNITIVE OR EXEMPLARY
//      DAMAGES ARISING OUT OF THIS LICENSE OR THE USE OF THE WORK OR OTHERWISE,
//      EVEN IF THE AUTHOR OR THE PUBLISHER HAS BEEN ADVISED OF THE POSSIBILITY OF
//      SUCH DAMAGES.
//   9. Termination.
//      1. This License and the rights granted hereunder will terminate
//         automatically upon any breach by You of any term of this License.
//         Individuals or entities who have received Derivative Works from You
//         under this License, however, will not have their licenses terminated
//         provided such individuals or entities remain in full compliance with
//         those licenses. Sections 1, 2, 6, 7, 8, 9, 10 and 11 will survive any
//         termination of this License.
//      2. If You bring a copyright, trademark, patent or any other infringement
//         claim against any contributor over infringements You claim are made by
//         the Work, your License from such contributor to the Work ends
//         automatically.
//      3. Subject to the above terms and conditions, this License is perpetual
//         (for the duration of the applicable copyright in the Work).
//         Notwithstanding the above, the Author reserves the right to release the
//         Work under different license terms or to stop distributing the Work at
//         any time; provided, however that any such election will not serve to
//         withdraw this License (or any other license that has been, or is
//         required to be, granted under the terms of this License), and this
//         License will continue in full force and effect unless terminated as
//         stated above.
//   10. Publisher. The parties hereby confirm that the Publisher shall not, under
//       any circumstances, be responsible for and shall not have any liability in
//       respect of the subject matter of this License. The Publisher makes no
//       warranty whatsoever in connection with the Work and shall not be liable to
//       You or any party on any legal theory for any damages whatsoever, including
//       without limitation any general, special, incidental or consequential
//       damages arising in connection to this license. The Publisher reserves the
//       right to cease making the Work available to You at any time without notice.
//   11. Miscellaneous
//       1. This License shall be governed by the laws of the location of the head
//          office of the Author or if the Author is an individual, the laws of
//          location of the principal place of residence of the Author.
//       2. If any provision of this License is invalid or unenforceable under
//          applicable law, it shall not affect the validity or enforceability of
//          the remainder of the terms of this License, and without further action
//          by the parties to this License, such provision shall be reformed to the
//          minimum extent necessary to make such provision valid and enforceable.
//       3. No term or provision of this License shall be deemed waived and no
//          breach consented to unless such waiver or consent shall be in writing
//          and signed by the party to be charged with such waiver or consent.
//       4. This License constitutes the entire agreement between the parties with
//          respect to the Work licensed herein. There are no understandings,
//          agreements or representations with respect to the Work not specified
//          herein. The Author shall not be bound by any additional provisions that
//          may appear in any communication from You. This License may not be
//          modified without the mutual written agreement of the Author and You.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal partial class Avi
	{
		public class Codec
		{
			private static readonly Codec[] _installedCodecs = LoadInstalledCodecs();

			private ICINFO _icInfo;
			private string _description = null;
			private string _name = null;
			private string _driver = null;
			private readonly string _fourCCCode;

			private Codec(ICINFO icInfo)
			{
				_icInfo = icInfo;
				_fourCCCode = mmioFourCCToString(FourCCHandler); 
			}

			public string FourCCCode
			{
				get { return _fourCCCode; }	
			}

			public int FourCCHandler
			{
				get { return _icInfo.fccHandler; }
			}

			public string Name
			{
				get
				{
					if (_name == null)
						_name = StringFromShortArray(_icInfo.szName);

					return _name;
				}
			}

			public string Description
			{
				get
				{
					if (_description == null)
						_description = StringFromShortArray(_icInfo.szDescription);

					return _description;
				}
			}

			public string Driver
			{
				get
				{
					if (_driver == null)
						_driver = StringFromShortArray(_icInfo.szDriver);

					return _driver;
				}
			}

			public bool SupportsQuality
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_QUALITY); }
			}

			public bool SupportsCrunching
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_CRUNCH); }
			}

			public bool SupportsDraw
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_DRAW); }
			}

			public bool WantsCompressAllFramesMessage
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_COMPRESSFRAMES); }
			}

			public bool SupportsTemporalCompression
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_TEMPORAL); }
			}

			public bool SupportsFastTemporalCompression
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_FASTTEMPORALC); }
			}

			public bool SupportsFastTemporalDecompression
			{
				get { return 0 != ((ICInfoFlags)_icInfo.dwFlags & ICInfoFlags.VIDCF_FASTTEMPORALD); }
			}

			public bool CanCompress(BITMAPINFOHEADER format)
			{
				return this == Find(format, this);
			}

			public static Codec Find(BITMAPINFOHEADER format, Codec preferredCodec)
			{
				ICModeFlags flags = ICModeFlags.ICMODE_COMPRESS | ICModeFlags.ICMODE_FASTCOMPRESS;
				Codec codec = Find(format, flags, preferredCodec);
				if (codec != null)
					return codec;

				flags = ICModeFlags.ICMODE_COMPRESS;
				codec = Find(format, flags, preferredCodec);
				if (codec != null)
					return codec;

				flags = 0;
				codec = Find(format, flags, preferredCodec);
				if (codec != null)
					return codec;

				return null;
			}

			public static Codec[] GetInstalledCodecs()
			{
				return _installedCodecs;
			}

			public static Codec GetInstalledCodec(string fccHandlerCode)
			{
				return CollectionUtils.SelectFirst(_installedCodecs,
					delegate(Codec codec)
					{
						return codec.FourCCCode == fccHandlerCode;
					});
			}

			public override string ToString()
			{
				return String.Format("{0} | {1} | {2}", Name, Description, Driver);
			}

			private static unsafe string StringFromShortArray(ushort[] source)
			{
				string value = "";
				if (source.Length == 0)
					return value;

				fixed (ushort* shortString = source)
				{
					char* charPtr = (char*)shortString;

					StringBuilder builder = new StringBuilder();
					for (int i = 0; i < source.Length; ++i)
					{
						char character = *charPtr++;
						if (character != '\0')
							builder.Append(character);
						else
							builder.Append("");
					}

					value = builder.ToString().Trim();
				}

				return value;
			}

			private static Codec Find(BITMAPINFOHEADER format, ICModeFlags flags, Codec preferredCodec)
			{
				int fccType = mmioStringToFOURCC("VIDC", 0);

				int preferredFccHandler = 0;
				if (preferredCodec != null)
					preferredFccHandler = preferredCodec.FourCCHandler;

				IntPtr handle = ICLocate(fccType, preferredFccHandler, ref format, IntPtr.Zero, (short)flags);
				GC.KeepAlive(format);

				if (handle != IntPtr.Zero)
				{
					ICINFO icInfo = new ICINFO();
					icInfo.dwSize = Marshal.SizeOf(icInfo);

					if (0 == ICGetInfo(handle, ref icInfo, icInfo.dwSize))
					{
						ICClose(handle);
						return null;
					}

					ICClose(handle);

					return CollectionUtils.SelectFirst(
						_installedCodecs, 
						delegate(Codec codec) { return codec.FourCCHandler == icInfo.fccHandler; });
				}

				return null;
			}

			private static Codec[] LoadInstalledCodecs()
			{
				List<Codec> codecs = new List<Codec>();

				int fccType = mmioStringToFOURCC("VIDC", 0);

				int i = 0;
				ICINFO handlerInfo = new ICINFO();
				handlerInfo.dwSize = Marshal.SizeOf(handlerInfo);

				while (0 != ICInfo(fccType, i++, ref handlerInfo))
				{
					IntPtr handle = ICOpen(fccType, handlerInfo.fccHandler, (Int32)ICModeFlags.ICMODE_QUERY);
					if (handle == IntPtr.Zero)
						continue;

					ICINFO queryHandlerInfo = new ICINFO();
					queryHandlerInfo.dwSize = Marshal.SizeOf(queryHandlerInfo);
					if (0 != ICGetInfo(handle, ref queryHandlerInfo, queryHandlerInfo.dwSize))
					{
						Codec codec = new Codec(queryHandlerInfo);
						if (!String.IsNullOrEmpty(codec.Name) &&
							null == codecs.Find(delegate(Codec test) { return test.Name == codec.Name; }))
						{
							codecs.Add(codec);
						}
					}

					ICClose(handle);
				}

				return codecs.ToArray();
			}
		}
	}
}