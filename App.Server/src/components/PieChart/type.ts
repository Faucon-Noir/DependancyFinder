export type SPEntity = {
	name: string;
	dependencies: SPEntity[];
	size: number;
	type: string;
	filePath: string;
	heavyQueries: Record<string, number>;
	gptReport: string;
	// LastUpdated: string;
};

export type DemoData = {
	name: string;
	dependencies: [];
};
