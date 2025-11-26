using CSM_Database_Testing.Disposing.Abstractions.Bases;

using Microsoft.EntityFrameworkCore;

namespace CSM_Database_Testing.Disposing;

/// <summary>
///     Implementation for a [Quality] purposes data [Disposition], data created to handle and simulate test/quality cases.
/// </summary>
public class TestingDisposer
    : TestingDisposerBase {

    /// <summary>
    ///     Creates a new <see cref="TestingDisposer"/> instance.
    /// </summary>
    /// <param name="Factories">
    ///     Subscribed <see cref="DbContext"/> handlers for data remotion.
    /// </param>
    public TestingDisposer(params DatabaseFactory[] Factories)
        : base(Factories) {
    }
}
