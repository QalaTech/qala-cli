class Qala < Formula
  desc "The official cli for Qala"
  homepage "https://github.com/QalaTech/Qala-Cli"
  url "file:///Users/davidturner/dev/qala-cli/src/Qala.Cli/bin/qala-cli-osx-arm64.v2.tar.gz"
  sha256 "af3fd063da3d97719e9695489804b593094f4e9604b10d24334f32f816f7933b"
  version "1.0.0"

  def install
    bin.install "qala"
  end

  test do
    system "#{bin}/qala", "--help"
  end
end