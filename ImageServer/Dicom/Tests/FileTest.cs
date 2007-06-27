#if UNIT_TESTS
using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom.Tests
{
    [TestFixture]
    public class FileTest
    {
        [Test]
        public void ConstructorTests()
        {
            DicomFile file = new DicomFile(null);

            file = new DicomFile("filename");

            file = new DicomFile(null, new AttributeCollection(), new AttributeCollection());


        }

        private void SetupMetaInfo(DicomFile theFile)
        {
            AttributeCollection theSet = theFile.MetaInfo;

            theSet[DicomTags.MediaStorageSOPClassUID].SetStringValue(theFile.DataSet[DicomTags.SOPClassUID].ToString());
            theSet[DicomTags.MediaStorageSOPInstanceUID].SetStringValue(theFile.DataSet[DicomTags.SOPInstanceUID].ToString());
            theFile.TransferSyntax = TransferSyntax.GetTransferSyntax(TransferSyntax.ExplicitVRLittleEndian); ;

            theSet[DicomTags.ImplementationClassUID].SetStringValue("1.1.1.1.1.11.1");
            theSet[DicomTags.ImplementationVersionName].SetStringValue("CC ImageServer 1.0");
        }

        private void SetupMR(AttributeCollection theSet)
        {
            theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
            theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
            theSet[DicomTags.InstanceCreationDate].SetStringValue("20070618");
            theSet[DicomTags.InstanceCreationTime].SetStringValue("133600");
            theSet[DicomTags.SOPClassUID].SetStringValue("1.2.840.10008.5.1.3.1.1.4");
            theSet[DicomTags.SOPInstanceUID].SetStringValue("1.1.1.1.1");
            theSet[DicomTags.StudyDate].SetStringValue("20070618");
            theSet[DicomTags.StudyTime].SetStringValue("1336000");
            theSet[DicomTags.SeriesDate].SetStringValue("20070618");
            theSet[DicomTags.SeriesTime].SetStringValue("133700");
            theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
            theSet[DicomTags.Modality].SetStringValue("MR");
            theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
            theSet[DicomTags.InstitutionName].SetStringValue("Mount Sinai Hospital");
            theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
            theSet[DicomTags.StudyDescription].SetStringValue("HEART");
            theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
            theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
            theSet[DicomTags.PatientID].SetStringValue("ID123-45-9999");
            theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
            theSet[DicomTags.PatientsSex].SetStringValue("M");
            theSet[DicomTags.PatientsWeight].SetStringValue("70");
            theSet[DicomTags.SequenceVariant].SetStringValue("OTHER");
            theSet[DicomTags.ScanOptions].SetStringValue("CG");
            theSet[DicomTags.MRAcquisitionType].SetStringValue("2D");
            theSet[DicomTags.SliceThickness].SetStringValue("10.000000");
            theSet[DicomTags.RepetitionTime].SetStringValue("857.142883");
            theSet[DicomTags.EchoTime].SetStringValue("8.712100");
            theSet[DicomTags.NumberofAverages].SetStringValue("1");
            theSet[DicomTags.ImagingFrequency].SetStringValue("63.901150");
            theSet[DicomTags.ImagedNucleus].SetStringValue("1H");
            theSet[DicomTags.EchoNumbers].SetStringValue("1");
            theSet[DicomTags.MagneticFieldStrength].SetStringValue("1.500000");
            theSet[DicomTags.SpacingBetweenSlices].SetStringValue("10.00000");
            theSet[DicomTags.NumberofPhaseEncodingSteps].SetStringValue("81");
            theSet[DicomTags.EchoTrainLength].SetStringValue("0");
            theSet[DicomTags.PercentSampling].SetStringValue("63.281250");
            theSet[DicomTags.PercentPhaseFieldofView].SetStringValue("68.75000");
            theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
            theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
            theSet[DicomTags.ProtocolName].SetStringValue("2D EPI BH");
            theSet[DicomTags.TriggerTime].SetStringValue("14.000000");
            theSet[DicomTags.LowRRValue].SetStringValue("948");
            theSet[DicomTags.HighRRValue].SetStringValue("1178");
            theSet[DicomTags.IntervalsAcquired].SetStringValue("102");
            theSet[DicomTags.IntervalsRejected].SetStringValue("0");
            theSet[DicomTags.HeartRate].SetStringValue("56");
            theSet[DicomTags.ReceiveCoilName].SetStringValue("B");
            theSet[DicomTags.TransmitCoilName].SetStringValue("B");
            theSet[DicomTags.InplanePhaseEncodingDirection].SetStringValue("COL");
            theSet[DicomTags.FlipAngle].SetStringValue("50.000000");
            theSet[DicomTags.PatientPosition].SetStringValue("HFS");
            theSet[DicomTags.StudyInstanceUID].SetStringValue("1.1.1.1.1.2");
            theSet[DicomTags.SeriesInstanceUID].SetStringValue("1.1.1.1.1.3");
            theSet[DicomTags.StudyID].SetStringValue("1933");
            theSet[DicomTags.SeriesNumber].SetStringValue("1");
            theSet[DicomTags.AcquisitionNumber].SetStringValue("7");
            theSet[DicomTags.InstanceNumber].SetStringValue("1");
            theSet[DicomTags.ImagePositionPatient].SetStringValue("-61.7564\\-212.04848\\-99.6208");
            theSet[DicomTags.ImageOrientationPatient].SetStringValue("0.861\\0.492\\0.126\\-0.2965");
            theSet[DicomTags.FrameofReferenceUID].SetStringValue("1.1.1.1.1.4");
            theSet[DicomTags.PositionReferenceIndicator].SetStringValue(null);
            theSet[DicomTags.ImageComments].SetStringValue("Test MR Image");
            theSet[DicomTags.SamplesperPixel].SetStringValue("1");
            theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
            theSet[DicomTags.Rows].SetStringValue("256");
            theSet[DicomTags.Columns].SetStringValue("256");
            theSet[DicomTags.PixelSpacing].SetStringValue("1.367188");
            theSet[DicomTags.BitsAllocated].SetStringValue("16");
            theSet[DicomTags.BitsStored].SetStringValue("12");
            theSet[DicomTags.HighBit].SetStringValue("11");
            theSet[DicomTags.PixelRepresentation].SetStringValue("0");
            theSet[DicomTags.WindowCenter].SetStringValue("238");
            theSet[DicomTags.WindowWidth].SetStringValue("471");

            uint length = 256 * 256 * 2;

            AttributeOW pixels = new AttributeOW(DicomTags.PixelData); ;

            byte[] pixelArray = new byte[length];

            for (uint i = 0; i < length; i+=2)
                pixelArray[i] = (byte)(i % 255);
            
            pixels.Values = pixelArray;

            theSet[DicomTags.PixelData] = pixels;

        }
        public void WriteOptionsTest(DicomFile sourceFile, DicomWriteOptions options)
        {
            bool result = sourceFile.Save(options);

            Assert.AreEqual(result, true);

            DicomFile newFile = new DicomFile("CreateFileTest.dcm");

            DicomReadOptions readOptions = DicomReadOptions.Default;
            newFile.Load(readOptions);

            Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);

            // update a tag, and make sure it shows they're different
            newFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
            Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), false);

            // Now update the original file with the name, and they should be the same again
            sourceFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
            Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);

            // Return the original string value.
            sourceFile.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Test");

        }

        [Test]
        public void CreateFileTest()
        {
            DicomFile file = new DicomFile("CreateFileTest.dcm");

            AttributeCollection dataSet = file.DataSet;

            AttributeCollection metaInfo = file.DataSet;


            SetupMR(dataSet);

            SetupMetaInfo(file);

            DicomWriteOptions writeOptions = DicomWriteOptions.Default;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.ExplicitLengthSequence;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.ExplicitLengthSequenceItem;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.None;
            WriteOptionsTest(file, writeOptions);

        }
    }
}
#endif