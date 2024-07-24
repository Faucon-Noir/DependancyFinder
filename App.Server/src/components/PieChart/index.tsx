import { useEffect, useState } from "react";
import { Gauge, PieChart } from "@mui/x-charts";
import json from "../../assets/output.json";
import { SPEntity } from "./type";
import { Box } from "@mui/material";
import { GaugeStyle, WrapperStyle } from "./style";

const CustomPieChart = () => {
	const [data, setData] = useState<any>([]);
	const jsonData: SPEntity = json;

	useEffect(() => {
		const fetchData = async () => {
			const adaptedData = jsonData.dependencies.map((item) => ({
				id: item.name,
				name: item.name,
				label: item.name
					.replace(/dbo\./g, "")
					.replace(/\.StoredProcedure/g, ""),
				dependencies: item.dependencies,
				value: item.size + 1,
			}));

			setData(adaptedData);
		};

		fetchData();
	}, []);

	return (
		<>
			<Box sx={{ marginBottom: 10 }}>
				Displaying {jsonData.name}'s data
				<br />
				<br />
				{jsonData.gptReport}
			</Box>
			<Box sx={WrapperStyle}>
				<Box sx={GaugeStyle}>
					Here are the dependencies:
					<PieChart
						sx={{ margin: 5 }}
						width={300}
						height={300}
						series={[
							{
								data: data,
								innerRadius: 80,
								outerRadius: 140,
								cx: 150,
								cy: 150,
								highlightScope: {
									faded: "global",
									highlighted: "item",
								},
								faded: {
									innerRadius: 100,
									additionalRadius: -20,
									color: "gray",
								},
							},
						]}
						slotProps={{ legend: { hidden: true } }}
					/>
					Hover items to see name & weight
				</Box>

				<Box sx={GaugeStyle}>
					Total :
					<Gauge
						width={200}
						height={100}
						value={jsonData.dependencies.length}
						startAngle={-90}
						endAngle={90}
					/>
					<br />
					(the fewer dependencies, the better)
				</Box>
			</Box>
		</>
	);
};

export default CustomPieChart;
