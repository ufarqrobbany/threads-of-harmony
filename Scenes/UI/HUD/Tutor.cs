// Scripts/UI/SequentialTextureDisplay.cs
using Godot;
using System;
using System.Threading.Tasks; // Untuk penggunaan Task.Delay

public partial class Tutor : Control
{
	// Export Path untuk TextureRects agar bisa di-assign dari editor
	[Export] public NodePath TextureRect1Path { get; set; }
	[Export] public NodePath TextureRect2Path { get; set; }
	[Export] public NodePath TextureRect3Path { get; set; }
	[Export] public NodePath TextureRect4Path { get; set; }
	[Export] public NodePath TextureRect5Path { get; set; }
	[Export] public NodePath TextureRect6Path { get; set; }

	[Export] public float DisplayDuration = 4.0f; // Durasi tampilan setiap TextureRect
	[Export] public float FadeDuration = 1.0f; // Durasi fade in/out

	private TextureRect[] textureRects;
	private int currentIndex = -1;

	public override void _Ready()
	{
		// Inisialisasi array TextureRects dari NodePath
		textureRects = new TextureRect[]
		{
			GetNode<TextureRect>(TextureRect1Path),
			GetNode<TextureRect>(TextureRect2Path),
			GetNode<TextureRect>(TextureRect3Path),
			GetNode<TextureRect>(TextureRect4Path),
			GetNode<TextureRect>(TextureRect5Path),
			GetNode<TextureRect>(TextureRect6Path)
		};

		// Sembunyikan semua TextureRects secara default
		foreach (var tr in textureRects)
		{
			if (tr != null)
			{
				tr.Modulate = new Color(tr.Modulate.R, tr.Modulate.G, tr.Modulate.B, 0); // Atur alpha ke 0 (transparan)
				tr.Visible = false;
			}
		}

		// Mulai urutan tampilan
		StartDisplaySequence();
	}

	private async void StartDisplaySequence()
	{
		// Tunggu sebentar sebelum memulai (opsional, sesuaikan sesuai kebutuhan)
		await Task.Delay(TimeSpan.FromSeconds(1));

		for (int i = 0; i < textureRects.Length; i++)
		{
			if (textureRects[i] != null)
			{
				currentIndex = i;
				await FadeInTextureRect(textureRects[i]);
				await Task.Delay(TimeSpan.FromSeconds(DisplayDuration - FadeDuration)); // Durasi tampilan dikurangi durasi fade in/out
				await FadeOutTextureRect(textureRects[i]);
			}
		}

		GD.Print("Semua TextureRects telah ditampilkan.");
		// Di sini Anda bisa menambahkan logika untuk apa yang terjadi setelah semua selesai,
		// misalnya memuat adegan baru, atau menyembunyikan kontrol ini.
		QueueFree(); // Contoh: Hapus node ini setelah selesai
	}

	private async Task FadeInTextureRect(TextureRect textureRect)
	{
		textureRect.Visible = true;
		float alpha = 0;
		while (alpha < 1)
		{
			alpha = Mathf.Min(alpha + (float)GetProcessDeltaTime() / FadeDuration, 1);
			textureRect.Modulate = new Color(textureRect.Modulate.R, textureRect.Modulate.G, textureRect.Modulate.B, alpha);
			await ToSignal(GetTree(), "process_frame"); // Tunggu frame berikutnya
		}
	}

	private async Task FadeOutTextureRect(TextureRect textureRect)
	{
		float alpha = 1;
		while (alpha > 0)
		{
			alpha = Mathf.Max(alpha - (float)GetProcessDeltaTime() / FadeDuration, 0);
			textureRect.Modulate = new Color(textureRect.Modulate.R, textureRect.Modulate.G, textureRect.Modulate.B, alpha);
			await ToSignal(GetTree(), "process_frame"); // Tunggu frame berikutnya
		}
		textureRect.Visible = false;
	}
}
