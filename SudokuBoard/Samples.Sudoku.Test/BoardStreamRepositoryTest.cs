﻿namespace Samples.Sudoku.Test
{
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Auth;
	using Microsoft.WindowsAzure.Storage.Blob;
	using Microsoft.WindowsAzure.Storage.Blob.Fakes;
	using Samples.Sudoku.Fakes;
	using System;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Tests for <see cref="Samples.Sudoku.BoardStreamRepository"/>
	/// </summary>
	[TestClass]
	public class BoardStreamRepositoryTest
	{
		[TestMethod]
		public async Task TestLoadBoard()
		{
			// A BoardStreamRepository needs an IStreamManager. Note that we use a
			// stub generated by Microsoft Fakes here.

			// Prepare
			var repository = BoardStreamRepositoryTest.SetupBoardStreamRepository(BoardSampleData.sampleBoard);

			// Execute
			var board = await repository.LoadAsync("DummyBoardName");

			// Assert
			Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual((byte[])board));
		}

		[TestMethod]
		public async Task TestLoadBoardFailures()
		{
			var repository = BoardStreamRepositoryTest.SetupBoardStreamRepository(new byte[] { 1, 2 });

			await AssertExtensions.ThrowsExceptionAsync<Exception>(
				async () => await repository.LoadAsync("DummyBoardName"));
		}

		[TestMethod]
		public async Task TestSaveBoard()
		{
			var buffer = new byte[9 * 9];
			var repository = BoardStreamRepositoryTest.SetupBoardStreamRepository(buffer);

			await repository.SaveAsync("DummyBoardName", (Board)BoardSampleData.sampleBoard);

			Assert.IsTrue(BoardSampleData.sampleBoard.SequenceEqual(buffer));
		}

		private static BoardStreamRepository SetupBoardStreamRepository(byte[] buffer)
		{
			var stub = new StubIStreamManager();
			stub.OpenStreamAsyncStringAccessMode = (_, __) =>
				Task.FromResult(new MemoryStream(buffer) as Stream);
			return new BoardStreamRepository(stub);
		}
	}
}
