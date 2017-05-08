
namespace LevelUpper.RNG.Crypto {

	public class EncDec {
		//public const byte encKey = 0x2B;
		public const long START = 0x5555555555555555; //unchecked((long)0xDEADCAFEBABEBEEF);
		const char EOT = (char)0x1F;

		long encPos;
		long decPos;


		public EncDec() {
			encPos = START;
			decPos = START;
		}

		public void Reset() {
			encPos = decPos = START;
		}


		/// <summary> Encrypts a segment of a Byte[], in place. </summary>
		/// <param name="message"> Byte[] to encrypt</param>
		/// <param name="index"> Index to begin encryption at. Negative numbers behave like 0. </param>
		/// <param name="length"> Length of Byte[] segment to encrypt. Negative numbers/0 encrypt from index to the end. </param>
		public void EncryptInPlace(byte[] message, int index = -1, int length = -1) {
			if (index < 0) { index = 0; }
			if (length < 1) { length = message.Length - index; }

			for (int i = 0; i < length; i++) {
				byte b = message[index + i];
				bool wasEOT = (b == EOT);
				long hash = SRNG.hash(encPos++);
				byte encDiff = (byte)hash;
				byte encKey = (byte)(hash >> 8);

				b += encDiff;
				b ^= encKey;

				message[index + i] = b;
				if (wasEOT) { encPos = START; }
			}
		}

		/// <summary> Decrypts a segment of a Byte[], in place. </summary>
		/// <param name="message"> Byte[] to decrypt </param>
		/// <param name="index"> Index to begin decryption at. Negative numbers behave like 0. </param>
		/// <param name="length"> Length of Byte[] segment to decrypt. Negative numbers/0 encrypt from index to the end. </param>
		public void DecryptInPlace(byte[] message, int index = -1, int length = -1) {
			if (index < 0) { index = 0; }
			if (length < 01) { length = message.Length - index; }

			for (int i = 0; i < length; i++) {
				byte b = message[index + i];
				long hash = SRNG.hash(decPos++);
				byte encDiff = (byte)hash;
				byte encKey = (byte)(hash >> 8);

				b ^= encKey;
				b -= encDiff;

				message[index + i] = b;
				if (b == EOT) { decPos = START; }
			}
		}

		/// <summary> Encrypts a message </summary>
		/// <param name="message"> Byte[] to encrypt </param>
		/// <param name="index"> Index to begin copy from. Negative numbers behave like 0. </param>
		/// <param name="length"> Length of Byte[] to copy. Negative numbers/0 take the rest of the array. </param>
		/// <returns> Encrypted copy of Byte[] segment </returns>
		public byte[] Encrypt(byte[] message, int index = -1, int length = -1) {
			if (index < 0) { index = 0; }
			if (length < 1) { length = message.Length - index; }

			byte[] enc = new byte[length];
			for (int i = 0; i < length; i++) {
				byte b = (message[index + i]);
				bool wasEOT = (b == EOT);
				long hash = SRNG.hash(encPos++);
				byte encDiff = (byte)hash;
				byte encKey = (byte)(hash >> 8);

				b += encDiff;
				b ^= encKey;

				enc[i] = b;
				if (wasEOT) { encPos = START; }
			}
			return enc;
		}

		/// <summary> Decrypts a message </summary>
		/// <param name="message"> Byte[] to decrypt </param>
		/// <param name="index"> Index to begin copy from. Negative numbers behave like 0. </param>
		/// <param name="length"> Length of characters to copy. Negative numbers/0 take the rest of the array. </param>
		/// <returns> Decrypted copy of byte[] segment </returns>
		public byte[] Decrypt(byte[] message, int index = -1, int length = -1) {
			if (index < 0) { index = 0; }
			if (length < 1) { length = message.Length - index; }

			byte[] dec = new byte[length];
			for (int i = 0; i < length; i++) {
				byte b = message[index + i];
				long hash = SRNG.hash(decPos++);
				byte encDiff = (byte)hash;
				byte encKey = (byte)(hash >> 8);

				b ^= encKey;
				b -= encDiff;

				dec[i] = b;
				if (b == EOT) { decPos = START; }
			}
			return dec;
		}

	}


}
